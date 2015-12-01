<Query Kind="Program" />

/* LINQPad script to load the Gutenberg Catalogue into a single, much smaller, csv file
 * containing just the basic book data needed by the Ebooks site. This is intended to
 * be run away from the server, on a desktop machine, to avoid decompressing and processing
 * 700MB+ of data on the server.
 * 
 * To update the catalogue, download it from http://www.gutenberg.org/cache/epub/feeds/rdf-files.tar.zip
 * Unzip the file and untar it to the desktop, then run this script. This will produce a Books.csv
 * with the minimal data from the catalogue, which can then be uploaded to the server and loaded into
 * the database using a GutCatalogueLoader object.
 */
void Main() {
	var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
	var docs = GetDocs(Path.Combine(desktop, "rdf-files"))
		.Where(f => f.Id.NotIn(0, 999999) && f.Title != null)
		// Don't include any that have no epubs (eg sound files)
		.Where(f => f.EpubUrlImages != null || f.EpubUrlNoImages != null);
	using (var csv = new StreamWriter(Path.Combine(desktop, "Books.csv"), false, Encoding.UTF8)) {
		docs.Select(d => new {
			d.Id,
			d.Title,
			d.Author,
			d.Language,
			StandardEpubUrlNoImages = d.StandardEpubUrlNoImages == d.EpubUrlNoImages,
			StandardEpubUrlImages = d.StandardEpubUrlImages == d.EpubUrlImages,
			EpubUrlNoImages = d.EpubUrlNoImages != null && d.EpubUrlNoImages != d.StandardEpubUrlNoImages ? d.EpubUrlNoImages : null,
			EpubUrlImages = d.EpubUrlImages != null && d.EpubUrlImages != d.StandardEpubUrlImages ? d.EpubUrlImages : null,
			StandardThumbnailUrl = d.StandardThumbnailUrl == d.ThumbnailUrl,
			StandardCoverUrl = d.StandardCoverUrl == d.CoverUrl,
			ThumbnailUrl = d.ThumbnailUrl != null && d.ThumbnailUrl != d.StandardThumbnailUrl ? d.ThumbnailUrl : null,
			CoverUrl = d.CoverUrl != null && d.CoverUrl != d.StandardCoverUrl ? d.CoverUrl : null
		}).ToCsv(csv);
	}
}

IEnumerable<GutCatDoc> GetDocs(string topfolder) {
	foreach (var f in Directory.GetFiles(topfolder, "*.rdf", SearchOption.AllDirectories)) {
		// Use yield so that only one at a time is loaded into memory.
		yield return new GutCatDoc(f);
	}
}

// Extracts data from a Gutenberg rdf file.
class GutCatDoc {
	private XDocument _doc;

	private static XNamespace _pgterms = "http://www.gutenberg.org/2009/pgterms/";
	private static XNamespace _cc = "http://web.resource.org/cc/";
	private static XNamespace _rdfs = "http://www.w3.org/2000/01/rdf-schema#";
	private static XNamespace _dcterms = "http://purl.org/dc/terms/";
	private static XNamespace _rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
	private static XNamespace _dcam = "http://purl.org/dc/dcam/";
	
	public GutCatDoc(string path) {
		_doc = XDocument.Load(path);
	}
	
	private XElement top => _doc.Root.Element(_pgterms + "ebook");
	
	public int Id => Convert.ToInt32(top.Attribute(_rdf + "about").Value.Substring(7));
	
	public string Author => top.Element(_dcterms + "creator")?.Descendants(_pgterms + "name")?.FirstOrDefault()?.Value;
	
	public string Title => top.Element(_dcterms + "title")?.Value;
	
	public string Language => top.Element(_dcterms + "language")?.Descendants(_rdf + "value").FirstOrDefault()?.Value;
	
	private IEnumerable<string> epubUrls => formats("application/epub+zip")
			?.Select(f => f.Element(_pgterms + "file")?.Attribute(_rdf + "about")?.Value);

	public string StandardEpubUrlNoImages => $"http://www.gutenberg.org/ebooks/{Id}.epub.noimages";

	public string StandardEpubUrlImages => $"http://www.gutenberg.org/ebooks/{Id}.epub.images";

	public string EpubUrlNoImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.noimages"));

	public string EpubUrlImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.images"));

	public IEnumerable<XElement> formats(string type) =>
		top.Elements(_dcterms + "hasFormat")?.Where(f => f.Element(_pgterms + "file")?.Element(_dcterms + "format")?.Element(_rdf + "Description")?.Element(_rdf + "value").Value == type);
			
	private IEnumerable<string> coverUrls => formats("image/jpeg")
		?.Select(f => f.Element(_pgterms + "file")?.Attribute(_rdf + "about")?.Value).Where(u => u.Contains("cover"));

	public string StandardThumbnailUrl => $"http://www.gutenberg.org/cache/epub/{Id}/pg{Id}.cover.small.jpg";

	public string StandardCoverUrl => $"http://www.gutenberg.org/cache/epub/{Id}/pg{Id}.cover.medium.jpg";

	public string ThumbnailUrl => coverUrls.SingleOrDefault(u => u.EndsWith("small.jpg"));

	public string CoverUrl => coverUrls.SingleOrDefault(u => u.EndsWith("medium.jpg"));
}

static class Extensions {
	public static string ToCsv<T>(this IEnumerable<T> source) {
		using (var m = new MemoryStream())
		using (var sw = new StreamWriter(m)) {
			ToCsv(source, sw);
			sw.Flush();
			m.Seek(0, SeekOrigin.Begin);
			using (var sr = new StreamReader(m)) {
				return sr.ReadToEnd();
			}
		}
	}

	public static void ToCsv<T>(this IEnumerable<T> source, StreamWriter output) {
		bool first = true;
		Regex re = new Regex(@"^\d+(|\.\d+)$");
		foreach (var element in source) {
			Type t = element.GetType();
			if (first) {
				var titles = t.GetProperties().Select(p => p.Name).ToArray();
				output.WriteLine(string.Join(",", titles));
				first = false;
			}
			var values = t.GetProperties().Select(p => {
				object o = p.GetValue(element, null);
				string val = o == null ? "" : o.ToString();
				if (o != null && !re.Match(val).Success) val = "\"" + val.Replace("\"", "\"\"") + "\"";
				return val;
			}).ToArray();
			output.WriteLine(string.Join(",", values));
		}
	}
}