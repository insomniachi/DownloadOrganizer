using DownloadOrganizer.Contracts;
using Moq;

namespace DownloadOrganizer.Tests
{
    public class MediaMoverTests
	{
		private const string DownloadsFolder = @"C:\Downloads";
		private const string MediaFolder = @"D:\Media";

		[Fact]
		public async Task IsAnimeTest()
		{
			var isAnime = await MalService.IsAnime("The Garden of Words");

			Assert.True(isAnime);
		}

		[Fact]
		public async Task MoverTrimsTamilMvFromName()
		{
			var movieFolder = Path.Combine(DownloadsFolder, "www 1TamilMV tf - CID Ramachandran Retd  SI (2024) Malayalam TRUE WEB-DL - 1080p - AVC - AAC - 2.9GB - ESub");
			var destinationFolder = Path.Combine(MediaFolder, "Movies - Malayalam", "CID Ramachandran Retd  SI (2024)");
			var movieName = "movie.mkv";
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.GetDirectories(DownloadsFolder)).Returns([movieFolder]);
			fileSystem.Setup(x => x.DirectoryExists(destinationFolder)).Returns(false);
			fileSystem.Setup(x => x.GetFiles(movieFolder)).Returns([Path.Combine(movieFolder, movieName)]);
			var mover = new MediaMover(fileSystem.Object);

			var movedFolder = await mover.MoveFolder(movieFolder, MediaFolder);

			Assert.Equal(destinationFolder, movedFolder);
			fileSystem.Verify(x => x.MoveFolder(movieFolder, destinationFolder), Times.Once);
		}

		[Fact]
		public async Task MovieFolder_MoviesToCorrectSubFolder()
		{
			var movieFolder = Path.Combine(DownloadsFolder, "A Quiet Place Part II (2020) [2160p] [4K][WEB] [HDR][5.1][YTS.MXJ]");
			var destinationFolder = Path.Combine(MediaFolder, "Movies", "A Quiet Place Part II (2020)");
			var movieName = "movie.mkv";
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.GetDirectories(DownloadsFolder)).Returns([movieFolder]);
			fileSystem.Setup(x => x.DirectoryExists(destinationFolder)).Returns(false);
			fileSystem.Setup(x => x.GetFiles(movieFolder)).Returns([Path.Combine(movieFolder, movieName)]);
			var mover = new MediaMover(fileSystem.Object);

			var movedFolder = await mover.MoveFolder(movieFolder, MediaFolder);

			Assert.Equal(destinationFolder, movedFolder);
			fileSystem.Verify(x => x.MoveFolder(movieFolder, destinationFolder), Times.Once);
		}

		[Fact]
		public async Task NewSeries_CreatesNewFolderAndMoves()
		{
			var seriesFolder = Path.Combine(DownloadsFolder, "Game of Thrones S08 COMPLETE 720p WEB-DL*264 ESubs[4GB][MP4][Season 8]");
			var destinationFolder = Path.Combine(MediaFolder, "Series", "Game of Thrones", "Season 8");
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.GetDirectories(DownloadsFolder)).Returns([seriesFolder]);
			fileSystem.Setup(x => x.DirectoryExists(destinationFolder)).Returns(false);
			fileSystem.Setup(x => x.GetFiles(seriesFolder)).Returns(
				[
					Path.Combine(seriesFolder, "1.mkv"),
					Path.Combine(seriesFolder, "2.mkv"),
					Path.Combine(seriesFolder, "3.mkv"),
					Path.Combine(seriesFolder, "4.mkv"),
					Path.Combine(seriesFolder, "5.mkv"),
					Path.Combine(seriesFolder, "6.mkv"),
					Path.Combine(seriesFolder, "7.mkv"),
				]);
			var mover = new MediaMover(fileSystem.Object);

			var movedFolder = await mover.MoveFolder(seriesFolder, MediaFolder);

			Assert.Equal(destinationFolder, movedFolder);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "1.mkv"), Path.Combine(destinationFolder, "1.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "2.mkv"), Path.Combine(destinationFolder, "2.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "3.mkv"), Path.Combine(destinationFolder, "3.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "4.mkv"), Path.Combine(destinationFolder, "4.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "5.mkv"), Path.Combine(destinationFolder, "5.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "6.mkv"), Path.Combine(destinationFolder, "6.mkv")), Times.Once);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(seriesFolder, "7.mkv"), Path.Combine(destinationFolder, "7.mkv")), Times.Once);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task ExistingSeriesWithYear_MovesNewEpisodeToCorrectFolder(bool existingFolderHasYear)
		{
			var episodeFolder = Path.Combine(DownloadsFolder, "House of the Dragon S02E05 1080p MAX WEB-DL DDP5 1 Atmos H 264-FLUX[TGx]");
			var destinationFolder = existingFolderHasYear
				? Path.Combine(MediaFolder, "Series", "House of the Dragon (2022)", "Season 2")
				: Path.Combine(MediaFolder, "Series", "House of the Dragon", "Season 2");

			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(x => x.GetDirectories(DownloadsFolder)).Returns([episodeFolder]);
			fileSystem.Setup(x => x.DirectoryExists(destinationFolder)).Returns(true);

			if(existingFolderHasYear)
			{
				fileSystem.Setup(x => x.GetDirectories(Path.Combine(MediaFolder, "Series"), "House of the Dragon (*)")).Returns([Path.Combine(MediaFolder, "Series", "House of the Dragon (2022)")]);
			}
			else
			{
				fileSystem.Setup(x => x.GetDirectories(Path.Combine(MediaFolder, "Series"), "House of the Dragon (*)")).Returns([]);
			}

			fileSystem.Setup(x => x.GetFiles(episodeFolder)).Returns(
				[
					Path.Combine(episodeFolder, "7.mkv"),
				]);
			var mover = new MediaMover(fileSystem.Object);
			
			var movedFolder = await mover.MoveFolder(episodeFolder, MediaFolder);
			
			Assert.Equal(destinationFolder, movedFolder);
			fileSystem.Verify(x => x.MoveFile(Path.Combine(episodeFolder, "7.mkv"), Path.Combine(destinationFolder, "7.mkv")), Times.Once);
		}
	}
}
