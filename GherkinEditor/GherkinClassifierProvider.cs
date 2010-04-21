using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace GherkinEditor
{
  [Export(typeof(IClassifierProvider))]
  [ContentType("feature")]
  internal class GherkinClassifierProvider : IClassifierProvider
  {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null;

    private static GherkinClassifier gherkinClassifier;

    public IClassifier GetClassifier(ITextBuffer textBuffer)
    {
      if (gherkinClassifier == null)
      {
        gherkinClassifier = new GherkinClassifier(ClassificationRegistry);
      }
      return gherkinClassifier;
    }
  }
}
