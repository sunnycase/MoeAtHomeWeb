
moeathomeApp.controller('viewBlogCtrl', ['$scope', '$http', '$routeParams',
    function ($scope, $http, $routeParams) {
        var date = $routeParams.date;
        var title = $routeParams.title;

        var getBlogUrl = function (date, title) {
            return '/api/blog/getBlog?date=' + date + "&title=" + title;
        }

        var highlight = function () {
            $('code').each(function (i, e) { hljs.highlightBlock(e) });
            $('.gutter').each(function (i, e) {
                var $thePre = $('pre', $(e).parent());
                var lineCount = $thePre.height() / parseFloat($thePre.css('line-height'));
                for (var i = 0; i < lineCount; i++) {
                    $(e).append("<div class='line-number'>" + (i + 1) + "</div>");
                }
            });
        };

        var getBlog = function () {
            $http({
                method: 'GET',
                url: getBlogUrl(date, title)
            }).success(function (data, status) {
                if (data != 'null') {
                    document.title = data.title + " - Moe@Home";
                    $scope.blog = data;
                    highlight();
                    $('#post').css('display', 'block');
                }
                else {
                    $('#post-notfound').css('display', 'block');
                }
            }).error(function (data, status) {
                // Some error occurred
            });
        };

        $scope.range = function (n) {
            return new Array(n);
        };

        $scope.onLoaded = function () {
            getBlog();
        };
    }]);