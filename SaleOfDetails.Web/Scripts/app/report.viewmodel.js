var ReportViewModel = function (app, dataModel) {
    var self = this;

    Sammy(function () {
        this.get('#reports', function () {
            app.markLinkAsActive('report');
            var year = 2016;

            $.ajax({
                method: 'get',
                url: '/api/Sale/ChartData/2016',
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (response) {
                    app.view(self);
                    self.showChart(response);
                }
            });
        });
    });

    self.showChart = function(data) {
        $('#chart').highcharts({
            credits: {
                enabled : false
            },
            title: {
                text: 'Продажи ОАО АТМ-Кубань',
                x: 0 //center
            },
            subtitle: {
                text: 'в 2016 году',
                x: 0
            },
            xAxis: {
                categories: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
                    'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь']
            },
            yAxis: {
                title: {
                    text: 'Сумма, руб.'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                formatter: function () {
                    return this.x + ': ' + this.point.y + ' руб.';
                },
                valueSuffix: ' руб.'
            },
            legend: {
                enabled: false
            },
            series: [{
                name: '',
                data: data
            }]
        });
    }

    return self;
}
 
app.addViewModel({
    name: "Report",
    bindingMemberName: "reports",
    factory: ReportViewModel
});