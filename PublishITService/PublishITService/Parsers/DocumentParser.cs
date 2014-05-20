using System;
using System.IO;

namespace PublishITService.Parsers {

    /// <summary>
    /// DocumentParser implements the IMediaParser-interface which has a method that stores a media object into a database. 
    /// </summary>
	public class DocumentParser : IMediaParser {

		private IPublishITEntities PublishItEntities { get; set; }

        /// <summary>
        /// Stores a media into a database.
        /// </summary>
        /// <param name="request">Object containing different info relevant for a media object.</param>
        /// <param name="entities">Interface for every entity class in the database.</param>
		public void StoreMedia(RemoteFileInfo request, IPublishITEntities entities)
		{
			this.PublishItEntities = entities;
			string path = @"\RentItServices\RentIt09\resources\media\document\" + request.UserId + @"\";
			Directory.CreateDirectory(Path.GetDirectoryName(path));

			FileStream targetStream = null;
			Stream sourceStream = request.FileStream;

			string filePath = Path.Combine(path, request.FileName);

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
			SaveMedia(filePath, request);
		}

		private void SaveMedia(string path, RemoteFileInfo request)
		{
			if (File.Exists(path)) {
				try {
					media media = new media {
						title = request.Title,
						format_id = 1,
						location = path,
						user_id = request.UserId
						
					};
					PublishItEntities.media.Add(media);
					PublishItEntities.SaveChanges();

					document document = new document {
						media_id = media.media_id,
						status = request.Status
					};
					PublishItEntities.document.Add(document);
					PublishItEntities.SaveChanges();

				} catch (Exception) {
					throw;
				}
			}
		}
	}
}