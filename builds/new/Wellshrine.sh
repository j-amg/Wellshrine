#!/bin/sh
echo -ne '\033c\033]0;Wellshrine\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Wellshrine.x86_64" "$@"
