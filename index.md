---
---
{% capture versions %}
  {% for pg in site.static_files %}
    {% assign segs = pg.path | split: '/' %}
    {% if segs[1] == 'v' %}
      {{ segs[2] }}
    {% endif %}
  {% endfor %}
{% endcapture %}
{% assign versions = versions | split: ' ' | uniq | sort | reverse %}

## API Reference

{% for version in versions %}
* [{{ version }}](v/{{ version }}/docs/)
  {% if version contains 'x' %}(latest){% endif %}
  {% include_relative v/{{version}}/spechash.txt %}
{% endfor %}
