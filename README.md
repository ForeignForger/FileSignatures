<p>This is a console app.</p>
<p>What does:
  Splits file in segments, generates segmets' SHA256 hashes and outputs them into console.</p>

**Console arguments:**
  - {File path to read}
  - {Segment size}
  - {Log buffer size(to output segments in batches)[skip or -1 for default: 1000]}
  - {Cooldown time in milliseconds. If the number of segments that were queued for processing is too big, we pause the file reading [skip or -1 for default: no cooldown time]}
  - {Max Segments queue size [skip or -1 for default: no max size]}
