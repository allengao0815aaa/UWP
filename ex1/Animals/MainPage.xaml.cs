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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Animals
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private delegate string AnimalSaying(object sender, EventArgs e);//声明一个委托
        private event AnimalSaying Say;//委托声明一个事件
        private bool FirstSpeaking = true;

        public MainPage()
        {
            this.InitializeComponent();
        }

        interface Animal
        {
            //方法
            string saying(object sender, EventArgs e);
            //属性
            int A { get; set; }
        }

        class pig : Animal
        {
            TextBlock word;
            private int a;

            public pig(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, EventArgs e)
            {
                this.word.Text += "pig: I am a pig\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class dog : Animal
        {
            TextBlock word;
            private int a;

            public dog(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, EventArgs e)
            {
                this.word.Text += "dog: I am a dog\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class cat : Animal
        {
            TextBlock word;
            private int a;

            public cat(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, EventArgs e)
            {
                this.word.Text += "cat: I am a cat\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        private pig p;
        private dog d;
        private cat c;

        private void speakButton_Click(object sender, RoutedEventArgs e)
        {
            if (FirstSpeaking)
            {
                FirstSpeaking = false;
                words.Text = "";
                p = new pig(words);
                d = new dog(words);
                c = new cat(words);
            }
            Random ran = new Random();
            int n = ran.Next(3);
            if (n == 0) Say = new AnimalSaying(p.saying);
            else if (n == 1) Say = new AnimalSaying(d.saying);
            else if (n == 2) Say = new AnimalSaying(c.saying);
            Say(this, EventArgs.Empty);
        }
        
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            if (FirstSpeaking)
            {
                FirstSpeaking = false;
                words.Text = "";
                p = new pig(words);
                d = new dog(words);
                c = new cat(words);
            }
            string str = textInput.Text;
            if (str == "pig") Say = new AnimalSaying(p.saying);
            else if (str == "dog") Say = new AnimalSaying(d.saying);
            else if (str == "cat") Say = new AnimalSaying(c.saying);
            if (str == "pig" || str == "dog" || str == "cat") Say(this, EventArgs.Empty);
            textInput.Text = "";
        }
    }
}
