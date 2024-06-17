using System.Text;

namespace EcommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh,string folder)
        {
            try
            {
                //GetCurrentDirectory địa chỉ project hiện tại của mình
                var fullpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myfile = new FileStream(fullpath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"dfhldladsfhlkldsaksfdhADFHKLAJDFLSDFKLDHLSFHLSF?><!";
            var sb = new StringBuilder();
            var rd=new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0,pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
