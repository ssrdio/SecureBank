class ChartTransactionHistory {
  constructor() {
    this.path = window.location.pathname;
    this.myLineChart = undefined;
    this.getChartData(this.fromDate, this.toDate);
  }

  charSuccess(data) {
    var recievedData = [];
    var recievedLabels = [];

    data.forEach(entry => {
      recievedLabels.push(entry.transactionDateTime);
      recievedData.push(entry.amount);
    });

    if (this.myLineChart && this.myLineChart != undefined) {
      this.myLineChart.destroy();
    }

    this.drawLineChart(recievedLabels, recievedData);
  }

  getChartData(from, to) {
    $.ajax({
      type: 'GET',
      url: '/api/Transaction/GetTransactionHistory?username=' + username,
      contentType: 'application/json',
      success: data => {
        this.charSuccess(data);
      }
      // dataType: dataType
    });
  }

  drawLineChart(labels, data) {
    var chartData = {
      labels: labels,
      datasets: [
        {
          label: 'Trasactions per day',
          data: data,
          backgroundColor: 'rgba(127,202,234,1)',
          borderColor: 'rgba(127,202,234,1)'
        }
      ]
    };

    var chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        yAxes: [
          {
            ticks: {
              beginAtZero: true
            }
          }
        ]
      }
    };

    var ctx = $('#myChart')
      .get(0)
      .getContext('2d');
    this.myLineChart = new Chart(ctx, {
      type: 'line',
      data: chartData,
      options: chartOptions
    });
  }
}
