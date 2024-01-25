# How to run

## Run with docker compose
```bash
docker compose up
```

## Open in browser
```
http://localhost:8080/swagger/index.html
```

# How to run db instance for local development
```bash
docker compose run -d -p 5432:5432 db
```

# TODO
- [ ] Add tests
- [ ] Add CI/CD
- [ ] Do not save duplicated responses from blockcypher
- [ ] Support scalability/realibility for fetcher service
- [ ] ...