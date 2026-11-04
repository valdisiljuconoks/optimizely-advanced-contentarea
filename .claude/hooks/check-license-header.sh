#!/usr/bin/env bash
# PostToolUse(Write|Edit): warn when a .cs file under src/ is missing the Apache-2.0 header.
f=$(jq -r '.tool_response.filePath // .tool_input.file_path' 2>/dev/null | tr -d '\r')
f=${f//\\//}

case "$f" in
  */obj/*|*/bin/*) exit 0 ;;
  */src/*.cs) ;;
  *) exit 0 ;;
esac

grep -q 'Copyright (c) Valdis Iljuconoks' "$f" 2>/dev/null && exit 0

printf '{"hookSpecificOutput":{"hookEventName":"PostToolUse","additionalContext":"%s is missing the two-line Apache-2.0 license header that every other file in src/ carries. Add it at the top of the file."}}' "$f"
