using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;

namespace GherkinEditor
{
  internal static class GherkinClassificationDefinitions
  {
    [Export]
    [Name("feature")]
    internal static ClassificationTypeDefinition featureClassificationDefinition = null;

    [Export]
    [Name("feature.scenario")]
    [BaseDefinition("feature")]
    internal static ClassificationTypeDefinition scenarioDefinition = null;

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "feature.scenario")]
    [Name("feature.scenario")]
    internal sealed class ScenarioFormat : ClassificationFormatDefinition
    {
      public ScenarioFormat()
      {
        ForegroundColor = Colors.Blue;
      }
    }
  }
}
