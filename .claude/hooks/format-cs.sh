#!/usr/bin/env bash
# PostToolUse(Write|Edit): apply .editorconfig whitespace rules to the edited C# file.
# `dotnet format --include` only matches paths RELATIVE to the working directory --
# an absolute path is silently ignored, so convert before calling it.
raw=$(jq -r '.tool_response.filePath // .tool_input.file_path' 2>/dev/null | tr -d '\r')

case "$raw" in
  *[/\\]obj[/\\]*|*[/\\]bin[/\\]*) exit 0 ;;
  *.cs) ;;
  *) exit 0 ;;
esac

f=$(cygpath -u "$raw" 2>/dev/null) || f=$raw
rel=${f#"$PWD"/}

dotnet format whitespace AdvancedContentArea.sln --include "$rel" --verbosity quiet 2>/dev/null || true
