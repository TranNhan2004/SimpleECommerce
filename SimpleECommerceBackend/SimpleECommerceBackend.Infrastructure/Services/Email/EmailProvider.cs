using System.Net;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

public class EmailProvider : IEmailProvider
{
    public string BuildAccountVerificationEmail(string verificationUrl)
    {
        var safeUrl = WebUtility.HtmlEncode(verificationUrl);

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Xác thực tài khoản</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color:#f4f4f4; padding:20px;"">
    <div style=""max-width:600px; margin:auto; background:#ffffff; padding:24px; border-radius:8px;"">
        <h2 style=""color:#333;"">Xác thực tài khoản</h2>
        <p>Chào bạn,</p>
        <p>Cảm ơn bạn đã đăng ký tài khoản.</p>
        <p>Vui lòng nhấn vào nút bên dưới để xác thực email của bạn:</p>

        <div style=""text-align:center; margin:30px 0;"">
            <a href=""{safeUrl}""
               style=""background-color:#2563eb; color:#ffffff; padding:12px 20px;
                      text-decoration:none; border-radius:6px; display:inline-block;"">
                Xác thực tài khoản
            </a>
        </div>

        <p>Nếu bạn không thực hiện đăng ký, vui lòng bỏ qua email này.</p>

        <p style=""margin-top:40px;"">Trân trọng,<br/>Đội ngũ hỗ trợ</p>
    </div>
</body>
</html>";
    }

    public string BuildPasswordResetEmail(string passwordResetUrl)
    {
        var safeUrl = WebUtility.HtmlEncode(passwordResetUrl);

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Đặt lại mật khẩu</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color:#f4f4f4; padding:20px;"">
    <div style=""max-width:600px; margin:auto; background:#ffffff; padding:24px; border-radius:8px;"">
        <h2 style=""color:#333;"">Yêu cầu đặt lại mật khẩu</h2>
        <p>Chào bạn,</p>
        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
        <p>Vui lòng nhấn vào nút bên dưới để thiết lập mật khẩu mới:</p>

        <div style=""text-align:center; margin:30px 0;"">
            <a href=""{safeUrl}""
               style=""background-color:#dc2626; color:#ffffff; padding:12px 20px;
                      text-decoration:none; border-radius:6px; display:inline-block;"">
                Đặt lại mật khẩu
            </a>
        </div>

        <p>Liên kết này có hiệu lực trong thời gian giới hạn. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>

        <p style=""margin-top:40px;"">Trân trọng,<br/>Đội ngũ hỗ trợ</p>
    </div>
</body>
</html>";
    }
}