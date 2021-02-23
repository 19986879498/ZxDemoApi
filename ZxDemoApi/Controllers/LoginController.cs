using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZxDemoCommon.Memory;

namespace ZxDemoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        public  IActionResult GetValidPicture()
        {
            string Code = GetvalidString(5);
            //获取图片
            byte[] Imgarr = this.CreateValidPicture(Code);
            //获取ip
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            //将验证码的值保存在缓存中
            MemoryCacheHelper.SetCache(ip, Code);

            return File(Imgarr,"Image/jpeg");
        }
        public  static string GetvalidString(int len)
        {
            string StrArr = "abcdefghijklmnopqrstuvwxyz123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            string ValidString = "";
            for (int i = 0; i < len; i++)
            {
                ValidString += StrArr[random.Next(StrArr.Length)];
            }
            return ValidString;
        }
        private   byte[] CreateValidPicture(string ValidCode)
        { 
            //创建画布
            Bitmap bitmap = new Bitmap(ValidCode.Length*24,40);
            //创建画笔
            Graphics graphics = Graphics.FromImage(bitmap);
            //给画布涂上背景色
            graphics.Clear(Color.White);
            //设置需要画到图中文字的格式（字体，大小，是否加粗。斜体）
            Font font = new Font("微软雅黑", 16, FontStyle.Bold | FontStyle.Italic);
            //设置颜料板
            RectangleF rf = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
            //把字画到画板中
            LinearGradientBrush brush = new LinearGradientBrush(rf,Color.Red,Color.DarkBlue,1.2f,true);
            graphics.DrawString(ValidCode,font,brush, 40,40);
            //将画板中的图片存到MemoryStream中
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            //释放画板
            bitmap.Dispose();
            Image image = SetTag(stream);
            string tagImg = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\imgs\\bbbtag.jpg";
           
            bitmap.Save(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\imgs\\bbb.jpg");
            return stream.ToArray();
          

        }

        private Image SetTag(Stream stream)
        {
            Image img = null;
            if (stream !=null)
            {
                img = Image.FromStream(stream);
            }
            //关联画笔
            Graphics graphics = Graphics.FromImage(img);
            //大小
            var size = img.Size;
            graphics.DrawString("hjw", new Font("微软雅黑", 16, FontStyle.Bold | FontStyle.Italic),Brushes.Blue,size.Width/2-size.Width/10/2,size.Height/2);
            return img;
        }

        [HttpPost]
        public IActionResult UploadImage(List<IFormFile> img)
        {
            if (img.Count==0)
            {
                return Ok("文件为空");
            }
            string imgUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","uploads");
            if (!Directory.Exists(imgUrl))
            {
                Directory.CreateDirectory(imgUrl);
            }
            const string filesType = ".bmp,.jpg,.png,.gif";
            foreach (var item in img)
            {
                if (item!=null)
                {
                    var fileext = Path.GetExtension(item.FileName);
                    if (fileext==null)
                    {
                        return Ok("后缀名有误");
                    }
                    if (filesType.ToLower().IndexOf(fileext.ToLower(),StringComparison.Ordinal)==-1)
                    {
                        return Ok("文件后缀名不支持图片");
                    }
                    long length = item.Length;
                    if (length>2048*1000)
                    {
                        return Ok("文件大小不得超过2M");
                    }
                    string Name = DateTime.Now.ToString("yyyyMMddHHmmss") + item.FileName;
                    Image img=SetTag
                    //using (FileStream fs = System.IO.File.Create(imgUrl +"\\"+ Name))
                    //{
                    //    item.CopyTo(fs);
                    //    fs.Flush();
                    //}
                }
            }
            return Ok("文件上传成功！");
        }
    }
}
