<Query Kind="Program" />

/* LINQPad script to load the Gutenberg Catalogue into a single, much smaller, xml file
 * containing just the basic book data needed by the Ebooks site. This is intended to
 * be run away from the server, on a desktop machine, to avoid decompressing and processing
 * 700MB+ of data on the server.
 * 
 * To update the catalogue, download it from http://www.gutenberg.org/cache/epub/feeds/rdf-files.tar.zip
 * Unzip the file and untar it to the desktop, then run this script. This will produce a Books.xml
 * with the minimal data from the catalogue, which can then be uploaded to the server and loaded into
 * the database using a GutCatalogueLoader object.
 *
 * Author doesn't get normalised out here as doing it made the file larger.
 */
void Main() {
	var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
	var docs = Directory.GetFiles(Path.Combine(desktop, "rdf-files"), "*.rdf", SearchOption.AllDirectories)
		.Select(f => new GutCatDoc(f)).Where(f => f.Id.NotIn(0, 999999) && f.Title != null);
	var outdoc = new XDocument(new XElement("Books"));
	foreach (var doc in docs) {
		var book = new XElement("Book",
							new XAttribute("Id", doc.Id),
							new XAttribute("Title", doc.Title),
							new XAttribute("Language", doc.Language));
		book.SetAttributeValue("Author", doc.Author);
		book.SetAttributeValue("EpubUrlNoImages", doc.EpubUrlNoImages);
		book.SetAttributeValue("EpubUrlImages", doc.EpubUrlImages);
		book.SetAttributeValue("ThumbnailUrl", doc.ThumbnailUrl);
		book.SetAttributeValue("CoverUrl", doc.CoverUrl);
		outdoc.Root.Add(book);
	}
	outdoc.Save(Path.Combine(desktop, "Books.xml"));
}

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
			
	public string EpubUrlNoImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.noimages"));

	public string EpubUrlImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.images"));

	public IEnumerable<XElement> formats(string type) =>
		top.Elements(_dcterms + "hasFormat")?.Where(f => f.Element(_pgterms + "file")?.Element(_dcterms + "format")?.Element(_rdf + "Description")?.Element(_rdf + "value").Value == type);
			
	private IEnumerable<string> coverUrls => formats("image/jpeg")
		?.Select(f => f.Element(_pgterms + "file")?.Attribute(_rdf + "about")?.Value).Where(u => u.Contains("cover"));

	public string ThumbnailUrl => coverUrls.SingleOrDefault(u => u.EndsWith("small.jpg"));

	public string CoverUrl => coverUrls.SingleOrDefault(u => u.EndsWith("medium.jpg"));
}