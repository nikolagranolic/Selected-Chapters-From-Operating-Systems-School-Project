using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TaskScheduler;

namespace TaskSchedulerDemo
{
    public class MinPoolingTask : IUserTask
    {
        public string Name { get; init; } = "MinPoolingTask";
        public string InputFolder { get; set; } = "..\\inFolderExample";
        public string OutputFolder { get; set; } = "..\\outFolderExample";
        public int KernelSize { get; set; } = 2;
        public int MaxDegreeOfParallelism { get; set; } = 1;

        public MinPoolingTask() { }

        public MinPoolingTask(string inputFolder, string outputFolder, int kernelSize, int maxDegreeOfParallelism)
        {
            this.InputFolder = inputFolder;
            this.OutputFolder = outputFolder;
            this.KernelSize = kernelSize;
            this.MaxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public void Run(ITaskContext taskApi)
        {
            DateTime start = DateTime.Now; // todelete

            Console.WriteLine($"{Name} started");

            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
            string[] files = Directory.GetFiles(InputFolder, "*", SearchOption.AllDirectories);

            List<String> input = new List<string>(files);
            List<String> inputFiles = new();
            foreach (var s in input)
            {
                inputFiles.Add(s.Replace("\\", "\\\\"));
            }

            Parallel.ForEach(inputFiles, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, (inputFile) =>
            {
                MemoryStream stream = new MemoryStream();

                using (FileStream fileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(stream);
                }

                stream.Position = 0;

                Bitmap inputImage = new Bitmap(stream);

                int width = inputImage.Width;
                int height = inputImage.Height;

                Bitmap outputImage = new Bitmap(width / KernelSize, height / KernelSize);

                var rect = new Rectangle(0, 0, width, height);
                var rect2 = new Rectangle(0, 0, width / KernelSize, height / KernelSize);
                var inData = inputImage.LockBits(rect, ImageLockMode.ReadOnly, inputImage.PixelFormat);
                var outData = outputImage.LockBits(rect2, ImageLockMode.WriteOnly, inputImage.PixelFormat);

                unsafe
                {
                    byte* inArray = (byte*)inData.Scan0;
                    byte* outArray = (byte*)outData.Scan0;

                    var bufferOut = new byte[inData.Width * inData.Height * 3];

                    Parallel.For(0, height / KernelSize, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
                    (y) =>
                    {
                        taskApi.Progress = y / (double)height / KernelSize;
                        int offset;
                        for (int x = 0; x < width; x += KernelSize)
                        {
                            int minValueR = 255;
                            int minValueG = 255;
                            int minValueB = 255;
                            int sum = 255 * 3;
                            for (int i = 0; i < KernelSize; i++)
                            {
                                for (int j = 0; j < KernelSize; j++)
                                {
                                    offset = (((y * KernelSize + i) * width) + x + j) * 3;
                                    var r = inArray[offset + 0];
                                    var g = inArray[offset + 1];
                                    var b = inArray[offset + 2];

                                    if ((r + g + b) < sum)
                                    {
                                        minValueR = r;
                                        minValueG = g;
                                        minValueB = b;
                                        sum = r + g + b;
                                    }
                                }
                            }
                            offset = ((y * width) + x) * 3 / KernelSize;
                            outArray[offset + 0] = (byte)minValueR;
                            outArray[offset + 1] = (byte)minValueG;
                            outArray[offset + 2] = (byte)minValueB;

                            taskApi.CheckForPause();
                        }
                    });
                    inputImage.UnlockBits(inData);
                    outputImage.UnlockBits(outData);
                }
                outputImage.Save(IUserTask.AddSuffix(Path.Combine(OutputFolder, Path.GetFileName(inputFile)), "_minpooled"), ImageFormat.Jpeg);
            });

            Console.WriteLine($"{Name} finished.");
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            Console.WriteLine(duration.ToString());
            taskApi.Progress = 1;
        }
    }
}

