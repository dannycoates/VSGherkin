using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace GherkinEditor
{
  internal static class GherkinContentDefinition
  {
    [Export]
    [Name("feature")]
    [BaseDefinition("text")]
    internal static ContentTypeDefinition featureContentTypeDefinition = null;

    [Export]
    [FileExtension(".feature")]
    [ContentType("feature")]
    internal static FileExtensionToContentTypeDefinition featureFileExtensionDefinition = null;
  }
}
