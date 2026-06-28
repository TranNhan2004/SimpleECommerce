#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$(cd "$SCRIPT_DIR/../../env" && pwd)/.env.prod"

if [ -f "$ENV_FILE" ]; then
  set -a
  source "$ENV_FILE"
  set +a
else
  echo "Missing .env file: $ENV_FILE"
  exit 1
fi

if [ -z "${TUNNEL_TOKEN:-}" ]; then
  echo "TUNNEL_TOKEN is empty or not set"
  exit 1
fi

exec cloudflared tunnel --no-autoupdate run --token "$TUNNEL_TOKEN"