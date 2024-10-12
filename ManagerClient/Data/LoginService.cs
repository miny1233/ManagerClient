using System.Buffers.Text;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using AntDesign.Filters;
using AspNetCore.Totp;
using QRCoder;

namespace ManagerClient.Data {
    public class LoginService{

        readonly static string secretKey = Guid.NewGuid().ToString();
        readonly TotpGenerator generator = new();
        // public bool HasOTP = false;

        private string otp = "";

        public LoginService()
        {
            // _ = CreateAndShowOTP();
        }

        public bool CheckOTP(string code)
        {
            return otp == code;
        }

       public void CreateAndShowOTP()
       {
            var code = generator.Generate(secretKey).ToString();

            if (otp == code)
            {
                Console.WriteLine("Keep OTP");
                return;
            }

            otp = code;

            Console.WriteLine("====================================");
            Console.WriteLine("Your New OTP Code is " + otp);
            Console.WriteLine("====================================");

            SendEmail(otp);
       }

       string[] AdminEmail = [
            "1844634677@qq.com",
            "1469445513@qq.com",
            "1596784863@qq.com"
        ];

       void SendEmail(string verifyCode)
       {
            string stmpServer = @"smtp.ncwu.store";
            string mailAccount = @"auth@ncwu.store";
            string pwd = @"12345678";

            SmtpClient smtpClient = new();

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //指定电子邮件发送方式
            smtpClient.Host = stmpServer; // 指定发送方SMTP服务器
            smtpClient.EnableSsl = true; // 使用安全加密连接
            smtpClient.UseDefaultCredentials = false; // 必须为false 否则邮件服务器不转发
            smtpClient.Credentials = new NetworkCredential(mailAccount, pwd);//设置发送账号密码

            foreach(var mailTo in AdminEmail) {
                MailMessage mailMessage = new(mailAccount, mailTo)
                {
                    Subject = "图书销售管理认证系统", // 设置发送邮件得标题
                    Body = "Your OTP Code is " + verifyCode,// 设置发送邮件内容
                    BodyEncoding = Encoding.UTF8, // 设置发送邮件得编码
                    IsBodyHtml = false, // 设置标题是否为HTML格式
                    Priority = MailPriority.Normal // 设置邮件发送优先级
                };

                try
                {
                    smtpClient.Send(mailMessage);//发送邮件
                    Console.WriteLine("Send email completed!");
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine("Exception occurred that send email to " + mailTo + "\n detail: \n" + ex.ToString());
                }
            }

       }
    }

}