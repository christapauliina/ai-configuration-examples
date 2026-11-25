# Power BI Sample Data

This folder contains the sample Power BI semantic model (`Penguins.pbix`) used by demo `08-AgentWithExternalMCP` (Power BI Modeling MCP server).

## Data Source
- **Dataset:** Palmer Penguins
- **Source:** [Seaborn sample datasets](https://github.com/mwaskom/seaborn-data/blob/master/penguins.csv) (originally published by Gorman, Williams & Fraser, 2014; CC0 / public domain)

The data is already embedded in `Penguins.pbix`, so no separate CSV file is needed.

## Setup
1. Install the [Power BI Modeling MCP server](https://github.com/microsoft/powerbi-modeling-mcp/releases) and set `POWERBI_MCP_PATH` in your `.env` file.
2. Open `Penguins.pbix` in Power BI Desktop.
3. Run the demo from the repository root:
   ```bash
   dotnet run --project 08-AgentWithExternalMCP
   ```
4. Try a prompt such as: *"What tables and measures exist in the report that is open locally?"* or *"Compare the average body mass across penguin species."*