using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Source.ImageScroller
{
    public class ImageScroller : ISimpleLed
    {
        private int scrollPos = 0;
        private LEDColor[,] imageAsGrid;
        private ControlDevice dev;
        public void Dispose()
        {
            
        }

        public void Configure(DriverDetails driverDetails)
        {
            
        }

        public List<ControlDevice> GetDevices()
        {
            dev = new ControlDevice
            {
                LEDs = new ControlDevice.LedUnit[23 * 6],
                DeviceType = DeviceTypes.Effect,
                Driver = this,
                GridHeight = 6,
                GridWidth = 23,
                Has2DSupport = true,
                Name = "Scroll Source"
            };

            int ct = 0;
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 23; x++)
                {
                    var led= new ControlDevice.LedUnit
                    {
                        Color = new LEDColor(0, 0, 0),
                        Data = new ControlDevice.PositionalLEDData
                        {
                            LEDNumber = ct,
                            X=x,
                            Y=y                                                   
                        },
                        LEDName = "POS " + ct
                    };

                    dev.LEDs[ct] = led;

                    ct++;
                }
            }

            return new List<ControlDevice>
            {
            dev    
            };
        }

        public void Push(ControlDevice controlDevice)
        {
            
        }

        private int scrollPosOffset = 0;
        public void Pull(ControlDevice controlDevice)
        {
            if (imageAsGrid == null)
            {
                imageAsGrid = GetImage("rainbowtext");
                scrollPos = 26;
            }

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < imageAsGrid.GetLength(0); x++)
                {
                    if (x + scrollPos < 23 && x + scrollPos >= 0)
                    {
                        dev.SetGridLED(x+scrollPos, y,imageAsGrid[x,y]);
                    }
                }
            }

            scrollPosOffset++;
          //  if (scrollPosOffset > 10)
            {
                scrollPosOffset = 0;

                scrollPos--;
                if (scrollPos < -imageAsGrid.GetLength(0))
                {
                    scrollPos = 26;
                }
            }
        }

        private void Clear()
        {
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 23; x++)
                {
                    dev.SetGridLED(x,y,new LEDColor(0,0,0));
                }
            }
        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                SupportsPull = true,
                SupportsPush = false,
                IsSource = false,
                Id = Guid.Parse("a9440d02-bba3-4e35-a9a3-28194acc0e2d"),
                Author = "mad ninja",
                Blurb = "Scroll images across your 2D devices",
                CurrentVersion = new ReleaseNumber(1, 0, 0, 1),
                GitHubLink = "https://github.com/SimpleLed/Source.ImageScroller",
                IsPublicRelease = true,
                SupportsCustomConfig = false
            };
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            
        }

        public string Name()
        {
            return "Image Scroller";
        }

        private LEDColor[,] GetImage(string path)
        {
            Bitmap image = Assembly.GetExecutingAssembly().GetEmbeddedImage("Source.ImageScroller." + path + ".png");

            float ratio = image.Height / 6f;
            int width = (int) (image.Width * ratio);

            Bitmap bm2 = new Bitmap(image, new Size(width, 6));

            LEDColor[,] result = new LEDColor[width,6];

            for (int y = 0; y < 6; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cl = bm2.GetPixel(x, y);
                    result[x, y] = new LEDColor(cl.R,cl.G,cl.B);
                }
            }

            return result;
        }
    }
}
