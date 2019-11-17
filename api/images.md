---
description: Image search
---

# Images

{% api-method method="get" host="https://api.miki.ai" path="/images" %}
{% api-method-summary %}
Get Images
{% endapi-method-summary %}

{% api-method-description %}
This lists all images ordered by latest images receieved.
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-headers %}
{% api-method-parameter name="Authentication" type="string" required=true %}
Bearer JWT that can be retrieved by emailing veld@miki.ai
{% endapi-method-parameter %}
{% endapi-method-headers %}

{% api-method-query-parameters %}
{% api-method-parameter name="page" type="string" required=false %}
Which page of results you want to retrieve, pages start at 0.
{% endapi-method-parameter %}

{% api-method-parameter name="tags" type="string" %}
Which tags the image need to contain, or which to filter. Example: `tags=animal+dog+-cat`
{% endapi-method-parameter %}
{% endapi-method-query-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```
{
    "id": 1169529585188999168,
    "tags": [
        "animal",
        "bird:lovebird",
        "bird"
    ],
    "url": "https://cdn.miki.ai/ext/imgh/1dGUZi4k8f.jpeg"
}
```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

{% api-method method="get" host="https://api.miki.ai" path="/images/:id" %}
{% api-method-summary %}
Get Image By ID
{% endapi-method-summary %}

{% api-method-description %}
Get one specific image from the API by ID.
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-path-parameters %}
{% api-method-parameter name="id" type="number" required=true %}
The snowflake ID of the image
{% endapi-method-parameter %}
{% endapi-method-path-parameters %}

{% api-method-headers %}
{% api-method-parameter name="Authentication" type="string" required=false %}
Bearer JWT that can be retrieved by emailing veld@miki.ai
{% endapi-method-parameter %}
{% endapi-method-headers %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```

```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}



