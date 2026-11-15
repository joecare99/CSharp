#!/usr/bin/env bash
set -euo pipefail

if (($# == 0)); then
  echo "Usage: bash tools/build/dotnet-isolated.sh <build|test|run|restore|publish|msbuild> [dotnet arguments...]" >&2
  exit 2
fi

command_name="$1"
shift
case "$command_name" in
  build|test|run|restore|publish|msbuild) ;;
  *)
    echo "Unsupported dotnet command: $command_name" >&2
    exit 2
    ;;
esac

for argument in "$@"; do
  if [[ "${argument,,}" == *customafterdirectorybuildprops* ]]; then
    echo "CustomAfterDirectoryBuildProps is reserved by the isolation wrapper." >&2
    exit 2
  fi
done

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "$script_dir/../.." && pwd)"
artifact_root="$repo_root/../../artifacts/linux"
mkdir -p "$artifact_root"

exec dotnet "$command_name" "-p:HostArtifactRoot=$artifact_root" \
  "-p:CustomAfterDirectoryBuildProps=$script_dir/IsolatedArtifacts.props" "$@"
