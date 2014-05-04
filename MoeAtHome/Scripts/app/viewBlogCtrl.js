
moeathomeApp.controller('viewBlogCtrl', ['$scope', '$http', '$routeParams', '$sce',
    function ($scope, $http, $routeParams, $sce) {
        var date = $routeParams.date;
        var title = $routeParams.title;

        var getBlogUrl = function (date, title) {
            return '/api/blog/getBlog?date=' + date + "&title=" + title;
        }

        var highlight = function () {
            $('pre').each(function (i, e) {
                hljs.highlightBlock(e);
                var $block = $('<div class="ui code segment"><table><tr><td class="gutter"/><td>'
                    + e.outerHTML + '</td></tr></table></div>');
                $(e).replaceWith($block);
            });
            $('.gutter').each(function (i, e) {
                var $thePre = $('pre', $(e).parent());
                var lineCount = $thePre.height() / parseFloat($thePre.css('line-height'));
                for (var i = 0; i < lineCount; i++) {
                    $(e).append("<div class='line-number'>" + (i + 1) + "</div>");
                }
            });
        };

        var renderContent = function (html) {
            $('.blog-content').html(html);
            highlight();
        };

        var getBlog = function () {
            $http({
                method: 'GET',
                url: getBlogUrl(date, title)
            }).success(function (data, status) {
                if (data != 'null') {
                    document.title = data.title + " - Moe@Home";
                    $scope.blog = data;
                    $('#post').css('display', 'block');
                    renderContent(data.content);
                }
                else {
                    document.title = "没找到这篇文章 - Moe@Home";
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