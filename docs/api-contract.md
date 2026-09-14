# API reference

Eight endpoints, all anonymous. Base URL is `http://localhost:7071` locally.

## Conventions

Menu requests and responses use the `MenuItem` entity shape. `PartitionKey` is
the category, `RowKey` is the item identifier. Responses are PascalCase.
Prices are `double`, because Azure Table Storage has no decimal type.

Errors are returned as plain text with the appropriate status code.

## Menu

### `POST /api/menu`

```json
{
  "partitionKey": "Hot Drinks",
  "rowKey": "COF-001",
  "name": "Espresso",
  "description": "Double shot, single origin.",
  "price": 22,
  "isAvailable": true
}
```

`partitionKey`, `rowKey` and `name` are required. `price` may not be negative.

| Status | Condition |
| --- | --- |
| 201 | Created. Returns the stored item. |
| 400 | Missing required fields, or a negative price. |

### `GET /api/menu`

200 with an array of items.

### `GET /api/menu/category/{category}`

200 with the items in that partition. An unmatched category returns an empty
array. Categories containing a space are percent-encoded: `Hot%20Drinks`.

### `PUT /api/menu/{category}/{id}`

```json
{
  "name": "Espresso",
  "description": "Double shot, single origin.",
  "price": 26.5,
  "isAvailable": false
}
```

Replaces name, description, price and availability.

| Status | Condition |
| --- | --- |
| 200 | Updated. Returns the item. |
| 400 | Negative price, or an unreadable body. |
| 404 | No such item in that category. |

### `DELETE /api/menu/{category}/{id}`

| Status | Condition |
| --- | --- |
| 200 | Deleted. |
| 404 | No such item in that category. |

## Documents

### `POST /api/documents/upload?fileName=`

The file is sent as the raw request body. `fileName` is a query parameter, and
its extension must be `.pdf`, `.doc`, `.docx` or `.txt`. The request
`Content-Type` becomes the stored blob's content type.

| Status | Condition |
| --- | --- |
| 201 | Stored. |
| 400 | Missing `fileName`, or a disallowed extension. |

### `GET /api/documents`

200 with an array of `FileName`, `Size`, `ContentType` and `LastModified`.

### `GET /api/documents/download/{fileName}`

| Status | Condition |
| --- | --- |
| 200 | The file bytes, with `Content-Type` and `Content-Disposition` set. |
| 404 | No such file. |

## Test data

The Postman collection creates and deletes `Hot Drinks / COF-001` on every run,
so `seed-menu.json` does not contain it. `Sandwiches` is left empty by the seed
data so the empty-category case has something to assert against.
