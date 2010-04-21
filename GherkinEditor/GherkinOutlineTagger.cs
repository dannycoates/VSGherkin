using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;

namespace GherkinEditor
{
  internal sealed class GherkinOutlineTagger : ITagger<IOutliningRegionTag>
  {
    private readonly string[] _startHide = { "Scenario:", "Feature:", "Background:" };
    private ITextBuffer _buffer;
    private ITextSnapshot _snapshot;
    private List<Region> _regions;

    public GherkinOutlineTagger(ITextBuffer buffer)
    {
      _buffer = buffer;
      _snapshot = buffer.CurrentSnapshot;
      _regions = new List<Region>();
      ReParse();
      buffer.Changed += BufferChanged;
    }

    void BufferChanged(object sender, TextContentChangedEventArgs e)
    {
      if (e.After != _buffer.CurrentSnapshot)
      {
        return;
      }
      ReParse();
    }

    private void ReParse()
    {
      var newSnapshot = _buffer.CurrentSnapshot;
      var newRegions = new List<Region>();

      Region currentRegion = null;
      var lastNonEmptyLine = 0;

      foreach (var line in newSnapshot.Lines)
      {
        var text = line.GetText().Trim();

        if (_startHide.Any(s => text.StartsWith(s)))
        {
          if (currentRegion != null)
          {
            currentRegion.EndLine = lastNonEmptyLine;
            currentRegion.Hover = newSnapshot.GetText(AsSnapshotSpan(currentRegion, newSnapshot));
            if (currentRegion.EndLine > currentRegion.StartLine)
            {
              newRegions.Add(currentRegion);
            }            
          }
          currentRegion = new Region
          {
            Text = text,
            StartLine = line.LineNumber
          };
        }
        else if (line.GetLineBreakText().Length == 0)
        {
          if (currentRegion != null)
          {
            var end = (text.Length > 0) ? lastNonEmptyLine + 1 : lastNonEmptyLine;
            currentRegion.EndLine = end;
            currentRegion.Hover = newSnapshot.GetText(AsSnapshotSpan(currentRegion, newSnapshot));
            newRegions.Add(currentRegion);
          }          
        }
        if (text.Length > 0)
        {
          lastNonEmptyLine = line.LineNumber;
        }
      }

      var oldSpans = new List<Span>(_regions.Select(r => AsSnapshotSpan(r, _snapshot)
        .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
        .Span));
      var newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));
      var oldSpanCol = new NormalizedSpanCollection(oldSpans);
      var newSpanCol = new NormalizedSpanCollection(newSpans);
      var removed = NormalizedSpanCollection.Difference(oldSpanCol, newSpanCol);
      var added = NormalizedSpanCollection.Difference(newSpanCol, oldSpanCol);

      var changeStart = int.MaxValue;
      var changeEnd = -1;

      if (removed.Count > 0)
      {
        changeStart = removed[0].Start;
        changeEnd = removed[removed.Count - 1].End;
      }

      if (added.Count > 0)
      {
        changeStart = Math.Min(changeStart, added[0].Start);
        changeEnd = Math.Max(changeEnd, added[added.Count - 1].End);
      }

      _snapshot = newSnapshot;
      _regions = newRegions;

      if (changeStart <= changeEnd)
      {
        if (TagsChanged != null)
        {
          TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_snapshot, Span.FromBounds(changeStart, changeEnd))));
        }
      }
    }

    private static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
    {
      var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
      var endLine = snapshot.GetLineFromLineNumber(region.EndLine);
      return new SnapshotSpan(startLine.Start, endLine.End);
    }

    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      if (spans.Count == 0)
      {
        yield break;
      }
      var currentRegions = _regions;
      var currentSnapshot = _snapshot;
      var entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End)
                    .TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
      var startLineNo = entire.Start.GetContainingLine().LineNumber;
      var endLineNo = entire.End.GetContainingLine().LineNumber;
      foreach (var region in currentRegions)
      {
        if (region.StartLine <= endLineNo && region.EndLine >= startLineNo)
        {
          var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
          var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);
          yield return new TagSpan<IOutliningRegionTag>(
            new SnapshotSpan(startLine.Start, endLine.End),
            new OutliningRegionTag(false, false, region.Text, region.Hover));
        }
      }
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
  }
}
