namespace D2L.Conversion.Converters.Blackboard.CPC.Common {

	public static class Constants {

		public const string IMS_MANIFEST_FILENAME = "imsmanifest.xml";
		public const string BB_PACKAGE_INFO_FILENAME = ".bb-package-info";
		public const string DROPBOX_ATTACHMENT_FOLDER = "_attachment_dropbox";

		public const string BlackboardNamespace = "http://www.blackboard.com/content-packaging/";

		public static class ManifestResourceType {
			public const string Announcement = "resource/x-bb-announcement";
			public const string Assignment = "resource/x-bb-assignment";
			public const string Discussion = "resource/x-bb-discussionboard";
			public const string Forumlink = "resource/x-bb-forumlink";
			public const string Gradebook = "course/x-bb-gradebook";
			public const string QtiPool = "assessment/x-bb-qti-pool";
			public const string QtiTest = "assessment/x-bb-qti-test";
			public const string RubricAssociation = "course/x-bb-crsrubricassocation";
			public const string Rubrics = "course/x-bb-rubrics";
		}
	}
}
