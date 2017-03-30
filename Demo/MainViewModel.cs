using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Demo
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Collection = new ObservableCollection<TestData>
            {
                new TestData
                {
                    Name = "Être",
                    Category = TestCategory.None,
                    Description = "If you be there",
                    GarconLevel = 18,
                    Children = new List<TestData>
                    {
                        new TestData
                        {
                            Name = "suis",
                            Category = TestCategory.Je,
                            GarconLevel = 13,
                            Description = "If I am there"
                        },
                        new TestData
                        {
                            Name = "es",
                            Category = TestCategory.Tu,
                            GarconLevel = 13,
                            Description = "If you are there"
                        },
                        new TestData
                        {
                            Name = "Multiple Options",
                            Category = TestCategory.None,
                            Description = "",
                            GarconLevel = -1,
                            Children = new List<TestData>
                            {
                                new TestData
                                {
                                    Name = "est",
                                    Category = TestCategory.Il,
                                    GarconLevel = 198,
                                    Description = "If he is there"
                                },
                                new TestData
                                {
                                    Name = "est",
                                    Category = TestCategory.Elle,
                                    GarconLevel = 198,
                                    Description = "If she is there"
                                },
                                new TestData
                                {
                                    Name = "est",
                                    Category = TestCategory.On,
                                    GarconLevel = 198,
                                    Description = "If he it there"
                                }
                            }
                        }
                    }
                }
            };

            for (int i = 0; i < 1000; i++)
            {
                Collection.Add(new TestData {Name = "Virtualizing Test"});
            }
        }

        public ObservableCollection<TestData> Collection { get; }
    }

    public class TestData
    {
        public string Name { get; set; }
        public TestCategory Category { get; set; }
        public string Description { get; set; }
        public int GarconLevel { get; set; }
        public List<TestData> Children { get; set; }
    }

    public enum TestCategory
    {
        None,
        Je,
        Tu,
        Il,
        Elle,
        On,
        Nous,
        Vous,
        IlsElles
    }
}