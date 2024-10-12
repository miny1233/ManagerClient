using System.Buffers.Text;
using System.Text.Encodings.Web;
using AspNetCore.Totp;
using QRCoder;

namespace ManagerClient.Data {
    public class LoginService{

        readonly static string secretKey = Guid.NewGuid().ToString();
        readonly TotpGenerator generator = new();
        public bool HasOTP = false;

        private string otp = "";

        public LoginService()
        {
            var code = CreateAndShowOTP();

            Console.WriteLine("====================================");
            Console.WriteLine("Your New OTP Code is " + code);
            Console.WriteLine("====================================");
        }

        public bool CheckOTP(string code)
        {
            return otp == code;
        }

       public string CreateAndShowOTP()
       {
            HasOTP = true;
            otp = generator.Generate(secretKey).ToString();
            return otp;
       }
    }

}