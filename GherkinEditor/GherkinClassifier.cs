using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;

namespace GherkinEditor
{
  public class GherkinClassifier : IClassifier
  {
    private IClassificationTypeRegistryService _classificationTypeRegistry;

    internal GherkinClassifier(IClassificationTypeRegistryService registry)
    {
      _classificationTypeRegistry = registry;
    }

    #pragma warning disable 67
    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    #pragma warning restore 67

    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
    {
      var snapshot = span.Snapshot;
      var spans = new List<ClassificationSpan>();

      if (snapshot.Length == 0)
      {
        return spans;
      }
      foreach (var line in snapshot.Lines)
      {
        var index = -1;
        var text = line.GetText();
        if ((index = text.IndexOf("Scenario:")) != -1)
        {
          var type = _classificationTypeRegistry.GetClassificationType("feature.scenario");
          spans.Add(new ClassificationSpan(new SnapshotSpan(snapshot, line.Start.Position + index, 8), type));
        }
      }
      return spans;
    }
  }
}
