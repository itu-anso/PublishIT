using System;
using System.IO;

namespace PublishITService.Parsers {
	public class VideoParser : IMediaParser {

		public IPublishITEntities _publishITEntities { get; set; }

		public void StoreMedia(byte[] mediaStream, RemoteFileInfo request, IPublishITEntities entities) {
			string path = @"rentit09/resources/media/video/" + request.FileName;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			try {
				using (var _FileStream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
					_FileStream.Write(mediaStream, 0, mediaStream.Length);
				}
			} catch (Exception) {

				throw;
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