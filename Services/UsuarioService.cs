using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
namespace Proyecto.Services
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hash seguro provicional para contraseña
        public string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // Crear admin inicial si no existe
        public async Task CrearAdminPorDefectoAsync()
        {
            if (!await _context.Usuarios.AnyAsync(u => u.Email == "juanlopezad25@gmail.com"))
            {
                var admin = new Usuario
                {
                    Email = "juanlopezad25@gmail.com",
                    Nombre = "Juan",
                    Rol = "Administrador",
                    PasswordHash = ComputeSha256Hash("admin")
                };
                _context.Usuarios.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Usuario?> ValidarLoginAsync(string email, string password)
        {
            // Protección contra valores nulos o vacíos
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            string hash = ComputeSha256Hash(password);

            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);
        }

        // Cambiar contraseña (faltaria implementarlo en usuario una vez logueado)
        public async Task CambiarPasswordAsync(string email, string nuevaPassword)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.PasswordHash = ComputeSha256Hash(nuevaPassword);
                await _context.SaveChangesAsync();
            }
        }

        // Genera token y guarda en DB
        public async Task<string> GenerarTokenRecuperacionAsync(string email)
        {
            var token = Guid.NewGuid().ToString("N");
            var tokenEntry = new PasswordResetToken
            {
                Token = token,
                Email = email,
                Expira = DateTime.UtcNow.AddMinutes(10) // expira en 10 minutos
            };
            _context.PasswordResetTokens.Add(tokenEntry);
            await _context.SaveChangesAsync();
            return token;
        }

        // Validar token
        public async Task<bool> ValidarTokenAsync(string token)
        {
            var tokenEntry = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (tokenEntry == null || tokenEntry.Expira < DateTime.UtcNow || tokenEntry.Usado)
                return false;

            return true;
        }

        // Obtener email por token
        public async Task<string?> ObtenerEmailPorTokenAsync(string token)
        {
            var tokenEntry = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            return tokenEntry?.Email;
        }


        // Marcar token como usado
        public async Task MarcarTokenUsadoAsync(string token)
        {
            var tokenEntry = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (tokenEntry != null)
            {
                tokenEntry.Usado = true;
                await _context.SaveChangesAsync();
            }
        }

        // Envío de email con MailKit
        public async Task EnviarEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Proyecto Adopción", "no-reply@xdominio.com"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("juanlopezad25@gmail.com", "ujfgspkbtytzwyor"); // usa contraseña de aplicación Gmail
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
