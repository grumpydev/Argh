namespace Argh.Tests
{
    using Xunit;

    public class ConfigTests
    {
        [Fact]
        public void Should_accept_just_filename()
        {
            var result = new Config(new[] { "myfilename.boo" });

            Assert.Equal("myfilename.boo", result.InputFile);
        }

        [Fact]
        public void Should_have_null_output_filename_when_not_specified()
        {
            var result = new Config(new[] { "myfilename.boo" });

            Assert.Null(result.OutputFile);
        }

        [Fact]
        public void Should_pickup_output_filename()
        {
            var result = new Config(new[] { "-o:test.csv", "myfilename.boo" });

            Assert.Equal("test.csv", result.OutputFile);
        }

        [Fact]
        public void Should_handle_output_filename_and_input_filename()
        {
            var result = new Config(new[] { "-o:test.csv", "myfilename.boo" });

            Assert.Equal("test.csv", result.OutputFile);
            Assert.Equal("myfilename.boo", result.InputFile);
        }
    }
}