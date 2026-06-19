// Loads and renders a completed test result.

$(function () {
    var resultId = window.resultId;
    if (!resultId) {
        $('#resultMessage').text('Không tìm thấy kết quả.');
        return;
    }

    $.getJSON('/Tests/GetResult', { id: resultId })
        .done(function (data) {
            $('#resultMessage').text('Bạn đã hoàn thành bài "' + (data.testTitle || '') + '". Dưới đây là gợi ý nghề nghiệp dành cho bạn.');
            $('#scoreDisplay').text(data.score);

            if (data.recommendedCareerPath) {
                var link = data.recommendedCareerPathId
                    ? '<a href="/CareerPaths/Details/' + data.recommendedCareerPathId + '">' + escapeResultHtml(data.recommendedCareerPath) + '</a>'
                    : escapeResultHtml(data.recommendedCareerPath);
                var compat = data.compatibilityScore != null ? ' (' + data.compatibilityScore + '% phù hợp)' : '';
                $('#careerPathDisplay').html(link + compat);
            } else {
                $('#careerPathDisplay').text('Chưa có gợi ý cụ thể.');
            }

            if (data.recommendedCareerPathId) {
                loadResultResources(data.recommendedCareerPathId);
            } else {
                $('#resourcesList').html('<p class="text-muted">Không có tài nguyên gợi ý.</p>');
            }
        })
        .fail(function () {
            $('#resultMessage').text('Không thể tải kết quả. Vui lòng thử lại.');
        });
});

function loadResultResources(careerPathId) {
    $.getJSON('/CareerPaths/GetDetails', { id: careerPathId })
        .done(function (data) {
            var list = $('#resourcesList');
            list.empty();
            if (!data.resources || data.resources.length === 0) {
                list.html('<p class="text-muted">Không có tài nguyên gợi ý.</p>');
                return;
            }
            var ul = $('<ul class="list-group"></ul>');
            data.resources.forEach(function (r) {
                var label = escapeResultHtml(r.title || 'Tài nguyên');
                var content = r.url
                    ? '<a href="' + r.url + '" target="_blank" rel="noopener">' + label + '</a>'
                    : label;
                ul.append('<li class="list-group-item">' + content + '</li>');
            });
            list.append(ul);
        })
        .fail(function () {
            $('#resourcesList').html('<p class="text-muted">Không thể tải tài nguyên.</p>');
        });
}

function escapeResultHtml(text) {
    if (text == null) return '';
    return $('<div></div>').text(text).html();
}
