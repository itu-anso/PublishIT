using System;
using System.IO;

namespace PublishITService.Parsers {
	public class VideoParser : IMediaParser {

		public IPublishITEntities _publishITEntities { get; set; }

		public void StoreMedia(Stream mediaStream, RemoteFileInfo request, IPublishITEntities entities) {
			string path = @"\RentItServices\RentIt09\resources\media\video\" + request.FileName;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			FileStream targetStream = null;
			Stream sourceStream = request.FileStream;

			string uploadFolder = @"C:\RentItServices\RentIt09\resources\media\document\1\";

			string filePath = Path.Combine(uploadFolder, request.FileName);

			using (targetStream = new FileStream(filePath, FileMode.Create,
								  FileAccess.Write, FileShare.None)) {
				//read from the input stream in 65000 byte chunks

				const int bufferLen = 65000;
				byte[] buffer = new byte[bufferLen];
				int count = 0;
				while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0) {
					// save to output stream
					targetStream.Write(buffer, 0, count);
				}
				targetStream.Close();
				sourceStream.Close();
			} 

			if (File.Exists(path)) {
				try {
					media media = new media {
						title = request.Title,
						format_id = 1,
						location = path,
						user_id = request.UserId
					};
					entities.media.Add(media);
					entities.SaveChanges();

					video video = new video {
						media_id = media.media_id,
						length = request.Length
					};
					entities.video.Add(video);
					entities.SaveChanges();

				} catch (Exception) {
					throw;
				}
			}
		}
	}
}