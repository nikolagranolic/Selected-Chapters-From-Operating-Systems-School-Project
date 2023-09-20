using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TaskScheduler;

namespace TaskSchedulerDemo
{
    public class AvgPoolingTask : IUserTask
    {
        public string Name { get; init; } = "AvgPoolingTask";
        public string InputFolder { get; set; } = "..\\inFolderExample";
        public string OutputFolder { get; set; } = "..\\outFolderExample";
        public int KernelSize { get; set; } = 2;
        public int MaxDegreeOfParallelism { get; set; } = 1;

        public AvgPoolingTask() { }

        public AvgPoolingTask(string inputFolder, string outputFolder, int kernelSize, int maxDegreeOfParallelism)
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

                    Parallel.For(0, height / KernelSize, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
                    (y) =>
                    {
                        taskApi.Progress = y / (double)height / KernelSize;
                        int offset;
                        for (int x = 0; x < width; x += KernelSize)
                        {
                            int sumR = 0;
                            int sumG = 0;
                            int sumB = 0;
                            for (int i = 0; i < KernelSize; i++)
                            {
                                for (int j = 0; j < KernelSize; j++)
                                {
                                    offset = (((y * KernelSize + i) * width) + x + j) * 3;
                                    sumR += inArray[offset + 0];
                                    sumG += inArray[offset + 1];
                                    sumB += inArray[offset + 2];
                                }
                            }

                            int avgR = sumR / (KernelSize * KernelSize);
                            int avgG = sumG / (KernelSize * KernelSize);
                            int avgB = sumB / (KernelSize * KernelSize);

                            offset = ((y * width) + x) * 3 / KernelSize;
                            outArray[offset + 0] = (byte)avgR;
                            outArray[offset + 1] = (byte)avgG;
                            outArray[offset + 2] = (byte)avgB;

                            taskApi.CheckForPause();
                        }
                    });
                    inputImage.UnlockBits(inData);
                    outputImage.UnlockBits(outData);
                }
                outputImage.Save(IUserTask.AddSuffix(Path.Combine(OutputFolder, Path.GetFileName(inputFile)), "_avgpooled"), ImageFormat.Jpeg);
            });
            Console.WriteLine($"{Name} finished.");
            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            Console.WriteLine(duration.ToString());
            taskApi.Progress = 1;
        }
    }
}

