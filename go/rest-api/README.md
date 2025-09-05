# 🚀 Quick Start

```bash
cd go/rest-api
go mod tidy
go run ./cmd/server
```

## Health & Hello
```bash
curl http://localhost:8080/health
curl "http://localhost:8080/hello?name=Anonym"
```

## Create
```bash
curl -X POST http://localhost:8080/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Min Flow","email":"minflow@example.com"}'
```

## List
```bash
curl http://localhost:8080/users
```

## Get (change {id} from result create/list)
```bash
curl http://localhost:8080/users/{id}
```

## Update
```bash
curl -X PUT http://localhost:8080/users/{id} \
  -H "Content-Type: application/json" \
  -d '{"name":"Min Flow v2"}'
```

## Delete
```bash
curl -X DELETE http://localhost:8080/users/{id}
```