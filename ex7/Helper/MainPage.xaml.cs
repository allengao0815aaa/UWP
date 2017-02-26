using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Xml;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Helper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        private async void GetPhoneNum()
        {
            try
            {
                // 创建一个HTTP client实例对象
                HttpClient httpClient = new HttpClient();

                // Add a user-agent header to the GET request. 
                /*
                默认情况下，HttpClient对象不会将用户代理标头随 HTTP 请求一起发送到 Web 服务。
                某些 HTTP 服务器（包括某些 Microsoft Web 服务器）要求从客户端发送的 HTTP 请求附带用户代理标头。
                如果标头不存在，则 HTTP 服务器返回错误。
                在 Windows.Web.Http.Headers 命名空间中使用类时，需要添加用户代理标头。
                我们将该标头添加到 HttpClient.DefaultRequestHeaders 属性以避免这些错误。
                */
                var headers = httpClient.DefaultRequestHeaders;

                // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
                // especially if the header value is coming from user input.
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }

                string getCode = "http://apistore.baidu.com/microservice/mobilephone?tel=" + value.Text;

                //发送GET请求
                HttpResponseMessage response = await httpClient.GetAsync(getCode);

                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

                // 可以用来测试返回的结果
                //string returnContent = await response.Content.ReadAsStringAsync();

                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                Encoding code = Encoding.GetEncoding("UTF-8");
                string result = code.GetString(getByte, 0, getByte.Length);

                JsonTextReader json = new JsonTextReader(new StringReader(result));
                string jsonVal = "";
                
                while (json.Read())
                {
                    jsonVal += json.Value;
                    if (jsonVal.Equals("province"))
                    {
                        json.Read();
                        textblock1.Text += json.Value;
                    }
                    if (jsonVal.Equals("carrier"))
                    {
                        json.Read();
                        textblock2.Text += json.Value;
                        break;
                    }
                    jsonVal = "";
                }
            }
            catch (HttpRequestException ex1)
            {
                textblock2.Text = ex1.ToString();
            }
            catch (Exception ex2)
            {
                textblock2.Text = ex2.ToString();
            }
        }

        private async void translate(int tag)
        {
            try
            {
                // 创建一个HTTP client实例对象
                HttpClient httpClient = new HttpClient();

                // Add a user-agent header to the GET request. 
                /*
                默认情况下，HttpClient对象不会将用户代理标头随 HTTP 请求一起发送到 Web 服务。
                某些 HTTP 服务器（包括某些 Microsoft Web 服务器）要求从客户端发送的 HTTP 请求附带用户代理标头。
                如果标头不存在，则 HTTP 服务器返回错误。
                在 Windows.Web.Http.Headers 命名空间中使用类时，需要添加用户代理标头。
                我们将该标头添加到 HttpClient.DefaultRequestHeaders 属性以避免这些错误。
                */
                var headers = httpClient.DefaultRequestHeaders;

                // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
                // especially if the header value is coming from user input.
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }

                string getCode;
                if (tag == 0) getCode = "http://fanyi.baidu.com/v2transapi?from=en&query=" + value.Text + "&to=zh";
                else getCode = "http://fanyi.baidu.com/v2transapi?from=zh&query=" + value.Text + "&to=en";

                //发送GET请求
                HttpResponseMessage response = await httpClient.GetAsync(getCode);

                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

                // 可以用来测试返回的结果
                //string returnContent = await response.Content.ReadAsStringAsync();

                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                Encoding code = Encoding.GetEncoding("UTF-8");
                string result = code.GetString(getByte, 0, getByte.Length);

                JsonTextReader json = new JsonTextReader(new StringReader(result));
                string jsonVal = "";

                while (json.Read())
                {
                    jsonVal += json.Value;
                    if (jsonVal.Equals("dst"))
                    {
                        json.Read();
                        textblock2.Text += json.Value;
                        break;
                    }
                    jsonVal = "";
                }
            }
            catch (HttpRequestException ex1)
            {
                textblock2.Text = ex1.ToString();
            }
            catch (Exception ex2)
            {
                textblock2.Text = ex2.ToString();
            }
        }

        private async void GetWeather()
        {
            // 创建一个HTTP client实例对象
            HttpClient httpClient = new HttpClient();

            // Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            // especially if the header value is coming from user input.
            string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            string getCode = "http://wthrcdn.etouch.cn/WeatherApi?city=" + value.Text;

            HttpResponseMessage response = await httpClient.GetAsync(getCode);

            // 确保返回值为成功状态
            response.EnsureSuccessStatusCode();

            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

            // 可以用来测试返回的结果
            //string returnContent = await response.Content.ReadAsStringAsync();

            // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
            Encoding code = Encoding.GetEncoding("UTF-8");
            string result = code.GetString(getByte, 0, getByte.Length);
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlNodeList listNodes = null;
            listNodes = doc.GetElementsByTagName("wendu");
            textblock2.Text += "温度： " + listNodes[0].ToString() + " ℃\n";
            listNodes = doc.GetElementsByTagName("fengli");
            textblock2.Text += "风力： " + listNodes[0].ToString() + "  ";
            listNodes = doc.GetElementsByTagName("fengxiang");
            textblock2.Text +=listNodes[0].ToString() + "\n";
            listNodes = doc.GetElementsByTagName("shidu");
            textblock2.Text += "湿度： " + listNodes[0].ToString() + "\n";
        }

        private void phoneNumButton_Click(object sender, RoutedEventArgs e)
        {
            title1.Text = "省份";
            title2.Text = "运营商";
            textblock1.Text = "";
            textblock2.Text = "";
            GetPhoneNum();
        }

        private void weatherButton_Click(object sender, RoutedEventArgs e)
        {
            title1.Text = "城市";
            title2.Text = "天气";
            textblock1.Text = "";
            textblock2.Text = "";
            GetWeather();
        }

        private void etocButton_Click(object sender, RoutedEventArgs e)
        {
            title1.Text = "原文";
            title2.Text = "翻译结果";
            textblock1.Text = value.Text;
            textblock2.Text = "";
            translate(0);
        }

        private void ctoeButton_Click(object sender, RoutedEventArgs e)
        {
            title1.Text = "原文";
            title2.Text = "翻译结果";
            textblock1.Text = value.Text;
            textblock2.Text = "";
            translate(1);
        }
    }
}
