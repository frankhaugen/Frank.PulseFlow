# Diagrams

Diagrams in this documentation use **Mermaid** where supported (GitHub, many Markdown preview tools, some wikis).

## Embedded in Markdown

| Document | Diagram type |
|----------|----------------|
| [Runtime model](../architecture/runtime-model.md) | Flowchart (components) |
| [Root README](../../README.md) | Multiple graphs (transmission / delivery / consumption) |

## Tips

- Prefer **relative links** from `docs/` back to the root README for the full illustrative set.
- For printable or offline docs, export Mermaid to SVG/PNG using your editor or [mermaid-cli](https://github.com/mermaid-js/mermaid-cli).

## Standalone snippets

You can paste the following into any Mermaid renderer to regenerate a high-level view:

```mermaid
flowchart LR
  P[Producers] --> IC[IConduit]
  IC --> CH[(Channel IPulse)]
  CH --> N[PulseNexus]
  N --> F[IFlow handlers]
```
