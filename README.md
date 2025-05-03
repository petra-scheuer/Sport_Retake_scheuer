# Sport_Retake_scheuer

1.) startung postgres on mac:
brew services start postgresql

2.) create database on mac: 
createdb Sports_Exercise_Battle

3.) in die PSQL Konsole wechseln:
psql -d Sports_Exercise_Battle

4.) in die Database gehen
\c Sports_Exercise_Battle

_____________________
curl tests:
curl -X POST http://localhost:10001/users \
     -H "Content-Type: application/json" \
     -d "{\"username\": \"testuser\", \"password\": \"testpass\"}"

curl -X POST http://localhost:10001/login \
     -H "Content-Type: application/json" \
     -d "{\"username\": \"testuser\", \"password\": \"testpass\"}"

#returns token

curl -X GET http://localhost:10001/history \
     -H "Content-Type: application/json" \
     -d "{\"username\": \"testuser\", \"Token\": \"8bab190e-9c30-4463-bcd7-1200c9d376ff\"}"

### Add a training
curl -X POST http://localhost:10001/history \
     -H "Content-Type: application/json" \
     -d "{\"Username\":\"testuser\", \"Token\":\"8bab190e-9c30-4463-bcd7-1200c9d376ff\", \"pushupcount\":10, \"duration\":120}"


curl -X GET http://localhost:10001/scoreboard \
     -H "Content-Type: application/json"

curl -X GET http://localhost:10001/my_user_stats \
     -H "Content-Type: application/json" \
     -d '{"Username":"testuser","Token":"8bab190e-9c30-4463-bcd7-1200c9d376ff"}'

