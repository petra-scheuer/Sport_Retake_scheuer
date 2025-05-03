#!/usr/bin/env bash
set -euo pipefail

SERVER="http://localhost:10001"
PLAYERS=(Petra Chris Teddy)
PASSWORD="SecretPass123"

for USER in "${PLAYERS[@]}"; do
  echo "=== Erstelle User $USER ==="
  # Returns plain text
  curl -s -X POST "$SERVER/users" \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"$USER\",\"password\":\"$PASSWORD\"}"
  echo -e "\n--- Login für $USER ---"
  # JSON {"Token":"…"}
  TOKEN=$(curl -s -X POST "$SERVER/login" \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"$USER\",\"password\":\"$PASSWORD\"}" \
    | jq -r '.Token')
  echo "Token: $TOKEN"

  # Direkt danach Training eintragen (plain text zurück)
  PUSHUPS=$(( RANDOM % 16 + 5 ))
  DURATION=$(( RANDOM % 121 + 60 ))
  echo "Training für $USER: $PUSHUPS Push-Ups, $DURATION s"
  curl -s -X POST "$SERVER/history" \
    -H "Content-Type: application/json" \
    -d "{
      \"Username\":\"$USER\",
      \"Token\":\"$TOKEN\",
      \"pushupcount\":$PUSHUPS,
      \"duration\":$DURATION
    }"
  echo -e "\n"
done

echo "=== Aktuelles Scoreboard (JSON) ==="
curl -s -X GET "$SERVER/scoreboard" \
  -H "Content-Type: application/json" \
  | jq .
