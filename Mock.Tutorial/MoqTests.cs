using System;
using Moq;
using NUnit.Framework;

namespace Mock.Tutorial
{
    public interface IMovieService
    {
        IMovie GetMovie(int id);

        void SaveMovie(IMovie movie);

        void DeleteMovie(IMovie movie);
    }

    public interface IMovie
    {
        int Id { get; set; }

        string Title { get; set; }
    }

    /// <summary>
    /// Diese Testklasse dient zu Veranschaulichung der verschiedenen Möglichkeiten, die uns Moq bietet und beinhaltet keine praxisnahen Tests.
    /// In den Tests wird der IMovieService getestet und dafür ein Mock erstellt.
    /// Alle Tests rufen immer die Methoden "GetMovie", "SaveMovie" und "DeleteMovie" auf.
    /// Die Methode "DeleteMovie" wird aufgerufen, es gibt aber KEIN entsprechendes Setup für die Methode.
    /// </summary>
    [TestFixture]
    public class MoqTests
    {
        readonly Mock<IMovie> movieMock = new Mock<IMovie>();

        [OneTimeSetUp]
        public void Setup()
        {
            this.movieMock.SetupGet(x => x.Id).Returns(9999);
            this.movieMock.SetupGet(x => x.Title).Returns("GOT");
        }

        /// <summary>
        /// TestCase "Default" und "Loose" laufen erfolgreich durch.
        /// TestCase "Strikt" läuft nicht durch, weil die Methode SaveMovie(null) nicht aufgerufen wird.
        /// </summary>
        /// <param name="mockBehavior"></param>
        [TestCase(MockBehavior.Default)] // Entspricht dem MockBehavior.Loose. Ergebnis: Erfolgreich!
        [TestCase(MockBehavior.Loose)] // Ergebnis: Der Test schläft fehl, weil SaveMovie(null) nicht aufgerufen wird.
        [TestCase(MockBehavior.Strict)] // Ergebnis: Erfolgreich!
        [Test]
        public void MockBehaviorTests(MockBehavior mockBehavior)
        {
            var movieServiceMock = CreateMovieServiceMock(mockBehavior);

            movieServiceMock.Object.SaveMovie(this.movieMock.Object);

            var movie = movieServiceMock.Object.GetMovie(movieMock.Object.Id);

            movieServiceMock.Object.DeleteMovie(movieMock.Object);

            Assert.AreEqual(movieMock.Object.Id, movie.Id);
            Assert.AreEqual(movieMock.Object.Title, movie.Title);
        }

        /// <summary>
        /// Dieser Test mit MockBehavior.Default und dem zusätzlichen Aufruf von "VerifyAll" verhält sich gleich wie MockBehavior.Strict.
        /// Damit es sich gleich verhält, muss immer "VerifyAll" aufgerufen werden.
        /// Ergebnis: Der Test schläft fehl, weil SaveMovie(null) nicht aufgerufen wird.
        /// </summary>
        [Test]
        public void VerifyTests()
        {
            var movieServiceMock = CreateMovieServiceMock();

            movieServiceMock.Object.SaveMovie(this.movieMock.Object);

            var movie = movieServiceMock.Object.GetMovie(movieMock.Object.Id);

            movieServiceMock.Object.DeleteMovie(movieMock.Object);

            Assert.AreEqual(movieMock.Object.Id, movie.Id);
            Assert.AreEqual(movieMock.Object.Title, movie.Title);

            movieServiceMock.VerifyAll();
        }

        private Mock<IMovieService> CreateMovieServiceMock(MockBehavior mockBehavior = MockBehavior.Default)
        {
            var movieServiceMock = new Mock<IMovieService>(mockBehavior);
            movieServiceMock.Setup(x => x.SaveMovie(null)).Throws<Exception>();
            movieServiceMock.Setup(x => x.GetMovie(movieMock.Object.Id)).Returns(movieMock.Object);
            return movieServiceMock;
        }
    }
}
