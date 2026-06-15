# API Gateway Configuration

## Development (Local)

Dùng file: `ocelot.json`
- Hosts: `localhost:5001`, `localhost:5002`, `localhost:5003`

Chạy:
```bash
cd backend
.\run.ps1
```

## Production (Docker)

Dùng file: `ocelot.json` (copy từ `ocelot.Docker.json`)
- Hosts: `contract-student-service:8080`, `billing-maintenance-service:8080`, `room-building-service:8080`

### Deploy lên server:

**Option 1: Copy toàn bộ nội dung từ ocelot.Docker.json**

```bash
# Trên server
ssh root@103.72.99.64
cd /opt/deploy/repo/student_dormitory_management_system/backend/APIGateway

# Backup file cũ
cp ocelot.json ocelot.json.backup

# Copy nội dung từ ocelot.Docker.json
# (Hoặc dùng editor để thay thế toàn bộ nội dung)
```

**Option 2: Chỉ thêm route /api/sepay (nếu đã có file đúng)**

Thêm đoạn này vào `Routes` (trước route cuối cùng):

```json
{
  "DownstreamPathTemplate": "/api/sepay/{everything}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "billing-maintenance-service",
      "Port": 8080
    }
  ],
  "UpstreamPathTemplate": "/api/sepay/{everything}",
  "UpstreamHttpMethod": ["Get", "Post", "Put", "Delete"]
},
```

### Restart container:

```bash
docker-compose restart api-gateway
# hoặc
docker restart api-gateway

# Check logs
docker logs -f api-gateway
```

## Sepay Webhook

**Development**: `http://localhost:5000/api/sepay/webhook`
**Production**: `http://ktxdnu.duckdns.org/api/sepay/webhook`

Cấu hình trên Sepay Dashboard:
- Webhook URL: `http://ktxdnu.duckdns.org/api/sepay/webhook`
- Account Number: `8871422018`
- API Key: `ktxdnu_sepay_2026`
