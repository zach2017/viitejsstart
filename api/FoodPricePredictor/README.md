
Here's how the food price prediction model works, explained with everyday examples:

Training Data & Features:
- Like learning from historical grocery receipts
- Features are like shopping factors: season (month), location (country), events (disasters)
- Example: Model learns beef prices rise more during droughts than bread prices

Model Type (SDCA - Stochastic Dual Coordinate Ascent):
- Regression model: Predicts continuous values (prices) vs classification (categories)
- Like a smart calculator that weighs multiple factors
- Example: Drought (0.5 severity) + Meat category (1.8x multiplier) = Higher price increase

Feature Processing:
1. Normalization
   - Converts different scales to same range (0-1)
   - Example: Converting $2.80 bread and $12.50 beef to comparable scales

2. One-Hot Encoding
   - Converts categories to numbers
   - Example: Meat=1, Dairy=0, Grain=0

Training Process:
1. Pattern Recognition
   - Learns relationships like "droughts affect meat prices more"
   - Similar to a grocery manager learning seasonal patterns

2. Model Evaluation
   - R² (accuracy): How well predictions match actual prices
   - RMSE (error margin): Average prediction error in dollars

Prediction:
- Takes new situation (user input)
- Applies learned patterns
- Example: If drought affected beef +30% historically, applies similar adjustment to new predictions

Here's how the transformer pipeline works in our food price model:

Pipeline Stages:
1. CopyColumns Transform
```csharp
.Append(mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Price"))
```
- Creates target variable (price) for training

2. Categorical Encoding
```csharp
.Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "FoodItemEncoded", inputColumnName: "FoodItem"))
```
- Converts text to numbers (Beef → [1,0,0], Chicken → [0,1,0])
- Essential for ML algorithms that only work with numbers

3. Numeric Feature Scaling
```csharp
.Append(mlContext.Transforms.NormalizeMinMax("MonthNorm", "Month"))
```
- Scales values to 0-1 range
- Example: Month 1-12 → 0.08-1.0

4. Feature Concatenation
```csharp
.Append(mlContext.Transforms.Concatenate("Features", "FoodItemEncoded", ...))
```
- Combines all processed features into single array
- Like organizing grocery factors into one spreadsheet row

The transformer pipeline prepares raw data into format suitable for ML training, similar to preparing ingredients before cooking.