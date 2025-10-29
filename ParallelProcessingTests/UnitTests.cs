using System;
using Xunit;
using ParallelProcessing;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelProcessing.Tests
{
    public class UnitTests
    {
        [Fact]
        public void MakeWidgets_Creates_Correct_Number()
        {
            var dict = new BespokeDictionary();
            var result = dict.MakeWidgets(100);
            Assert.True(result);
            Assert.Equal(100, dict.widgets.Count);
            Assert.True(dict.widgets.ContainsKey("X0000000099"));
        }

        [Fact]
        public void MakeWidgets_Zero_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(0);
            Assert.Equal(0, dict.widgets.Count);
        }

        [Fact]
        public void MakeWidgets_Negative_Size_Produces_Zero()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(-5);
            Assert.NotNull(dict.widgets);
            Assert.Equal(0, dict.widgets.Count);
        }

        [Fact]
        public void AddVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(10);
            var firstKey = dict.widgets.Keys.First();
            var before = dict.widgets[firstKey].Price;
            int changed = dict.AddVat(0.1);
            Assert.Equal(10, changed);
            var after = dict.widgets[firstKey].Price;
            Assert.Equal(Math.Round(before * 1.1, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_ZeroRate_NoChange()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            int changed = dict.AddVat(0.0);
            Assert.Equal(5, changed);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_NegativeRate_Decreases_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(6);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            int changed = dict.AddVat(-0.5);
            Assert.Equal(6, changed);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 0.5, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddVat_On_Empty_Dictionary_Returns_Zero()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            int changed = dict.AddVat(0.3);
            Assert.Equal(0, changed);
        }

        [Fact]
        public void AddIndexedSingleVat_Works_For_Valid_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = "X0000000002";
            var before = dict.widgets[key].Price;
            dict.AddIndexedSingleVat(2, 0.5);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.5, 10), Math.Round(after, 10));
        }

        [Fact]
        public void AddIndexedSingleVat_No_Throw_For_Invalid_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(3);
            // should not throw
            dict.AddIndexedSingleVat(10, 0.2);
        }

        [Fact]
        public void AddIndexedSingleVat_No_Throw_For_Negative_Index()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(3);
            var ex = Record.Exception(() => dict.AddIndexedSingleVat(-1, 0.2));
            Assert.Null(ex);
        }

        [Fact]
        public async Task ParralelForVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(20);
            int returned = await Task.Run(() => dict.ParralelForVat(0.1));
            Assert.Equal(20, returned);
        }

        [Fact]
        public void ParralelForVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(8);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            dict.ParralelForVat(0.2);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.2, 10), Math.Round(after, 10));
        }

        [Fact]
        public async Task ParralelForVat_On_Empty_Dictionary_Returns_Zero()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            int returned = await dict.ParralelForVat(0.1);
            Assert.Equal(0, returned);
        }

        [Fact]
        public async Task ParralelForOptionsVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(12);
            var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            int returned = await dict.ParralelForOptionsVat(options, 0.1);
            Assert.Equal(12, returned);
        }

        [Fact]
        public async Task ParralelForEachVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(15);
            int returned = await dict.ParralelForEachVat(0.2);
            Assert.Equal(15, returned);
        }

        [Fact]
        public void ParralelForEachVat_Updates_Prices()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(5);
            var key = dict.widgets.Keys.First();
            var before = dict.widgets[key].Price;
            dict.ParralelForEachVat(0.3);
            var after = dict.widgets[key].Price;
            Assert.Equal(Math.Round(before * 1.3, 10), Math.Round(after, 10));
        }

        [Fact]
        public async Task ParralelForEachOptionsVat_Returns_Size()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(7);
            var options = new ParallelOptions { MaxDegreeOfParallelism = 3 };
            int returned = await dict.ParralelForEachOptionsVat(options, 0.2);
            Assert.Equal(7, returned);
        }

        [Fact]
        public void AddSingleVat_Updates_Pair()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(4);
            var pair = dict.widgets.First();
            var before = pair.Value.Price;
            dict.AddSingleVat(pair, 0.25);
            Assert.Equal(Math.Round(before * 1.25, 10), Math.Round(dict.widgets[pair.Key].Price, 10));
        }

        [Fact]
        public void AddSingleVat_No_Throw_For_Missing_Pair()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            var missingPair = new KeyValuePair<string, Widget>("NoKey", new Widget { Id = 1, Name = "A", Price = 1.0 });
            var ex = Record.Exception(() => dict.AddSingleVat(missingPair, 0.1));
            Assert.Null(ex);
        }

        [Fact]
        public void AddSingleVat_Null_Value_Throws_NullReferenceException()
        {
            var dict = new BespokeDictionary();
            dict.widgets = new ConcurrentDictionary<string, Widget>();
            // insert a key with null value to simulate bad data
            dict.widgets.TryAdd("X0000000000", null);
            var pair = new KeyValuePair<string, Widget>("X0000000000", null);
            var ex = Record.Exception(() => dict.AddSingleVat(pair, 0.1));
            Assert.NotNull(ex);
            Assert.IsType<NullReferenceException>(ex);
        }

        [Fact]
        public void AddVatSpecific_Works()
        {
            var dict = new BespokeDictionary();
            dict.MakeWidgets(6);
            var before = dict.widgets["X0000000003"].Price;
            dict.AddVatSpecific(0.5);
            Assert.Equal(Math.Round(before * 1.5, 10), Math.Round(dict.widgets["X0000000003"].Price, 10));
        }

        [Fact]
        public void RandomString_Is_Correct_Length()
        {
            var dict = new BespokeDictionary();
            var method = dict.GetType().GetMethod("RandomString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)method.Invoke(dict, new object[] { 5 });
            Assert.Equal(5, result.Length);
        }
    }
}
