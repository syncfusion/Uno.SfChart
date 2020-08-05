# Syncfusion Uno Chart Control
**SfChart** provides a perfect way to visualize data with a high level of user interactivity that focuses on development, productivity and simplicity of use. **SfChart** also provides a wide variety of charting features that are used to visualize large quantities of data, flexible data binding and user customization.

<img width="700" src="/Images/SyncfusionUnoChart.gif">

## Features

Check out the feature list of our first Uno Chart below:

- Visualize and analyze data with ever-expanding 10+ charts and graphs ranging from line to pie charts.
- Interact with and explore charts with features such as zoom and pan, tooltip, and data labels.

### Chart Types

The Uno Chart control includes functionality for plotting more than 10 chart types. Each chart type is easily configured with built-in support for creating stunning visual effects.

- **Basic Charts**
    - Column Chart
    - Bar Chart
    - Line Chart
    - Spline Chart
    - Area Chart
    - SplineArea Chart
    - Area Chart
- **Correlation Charts**
    - Scatter Chart
    - Bubble Chart
- **Circular Charts**
    - Pie Chart
    - Doughnut Chart
    - Semi PieChart
    - Semi DoughnutChart
    - Stacked DoughnutChart

### Chart axis

The Uno Chart control supports three different types of axes: numerical, categorical, and date-time. The appearance of all chart axis elements can be customized with built-in properties.

### Data Labels

Data points can be easily annotated with labels to help improve the readability of data.

### Legends

Legends provide additional information that is helpful in identifying individual series in a chart. They can be docked to the left, right, top, or bottom positions around the chart area.

### Interactive Features

The end-user experience is greatly enhanced by a set of user interaction features: zooming, panning, and tooltip.

## Getting started

This section explains how to add the Uno Charts to your application and use its basic features.

**Step 1**
Download the Syncfusion Chart Uno Source here.

**Step 2**
Add Syncfusion.SfChart.Uno source project into your application solution.

**Step 3**
Refer the source to all platforms project.

**Step 4**
Import the SfChart namespace as shown below in your respective Page,
```xml
xmlns:syncfusion="using:Syncfusion.UI.Xaml.Charts"
```
**Step 5**
Then initialize an empty chart with two axes as shown below,
```xml
<syncfusion:SfChart> 
      <syncfusion:SfChart.PrimaryAxis>
           <syncfusion:CategoryAxis /> 
      </syncfusion:SfChart.PrimaryAxis> 
      <syncfusion:SfChart.SecondaryAxis>
           <syncfusion:NumericalAxis /> 
      </syncfusion:SfChart.SecondaryAxis>
</syncfusion:SfChart>
```
**Step 6** 

**Populate Chart with data** - As we are going to visualize the comparison of heights in the data model, add ColumnSeries to SfChart.Series property, and then bind the Data property of the ViewModel to the ColumnSeries.ItemsSource property and You need to set XBindingPath and YBindingPath properties, so that SfChart would fetch values from the respective properties in the data model to plot the series.
  
```xml
<syncfusion:SfChart Header="Getting Started for Uno Chart" Height="300" Width="500">
    <syncfusion:SfChart.PrimaryAxis>
        <syncfusion:CategoryAxis Header="Name" />
    </syncfusion:SfChart.PrimaryAxis>
    <syncfusion:SfChart.SecondaryAxis>
        <syncfusion:NumericalAxis Header="Height(in cm)" />
    </syncfusion:SfChart.SecondaryAxis>
    <syncfusion:ColumnSeries  ItemsSource="{Binding Data}" XBindingPath="Name" YBindingPath="Height" >
          <syncfusion:ColumnSeries.AdornmentsInfo>
              <syncfusion:ChartAdornmentInfo ShowLabel="True"  FontSize="16" LabelPosition="Inner" Foreground="White" />
          </syncfusion:ColumnSeries.AdornmentsInfo>
   </syncfusion:ColumnSeries>
</syncfusion:SfChart>
```
The following chart is created as a result of the above codes.

<img width="400" src="/Images/GettingStarted.png">

## Examples Application

You will also find a Visual Studio solution with the Syncfusion SampleBrowser.SfChart application. It demonstrates some basic chart features. Here are outputs of the application below:

<img width="700" src="/Images/SyncfusionChartDemo.gif">
