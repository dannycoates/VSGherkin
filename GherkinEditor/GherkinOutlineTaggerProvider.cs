using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace GherkinEditor
{
  [Export(typeof(ITaggerProvider))]
  [TagType(typeof(IOutliningRegionTag))]
  [ContentType("feature")]
  public sealed class GherkinOutlineTaggerProvider : ITaggerProvider
  {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    {
      Func<ITagger<T>> sc = () => new GherkinOutlineTagger(buffer) as ITagger<T>;
      return buffer.Properties.GetOrCreateSingletonProperty(sc);
    }
  }
}
