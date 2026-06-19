// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Loads category, learning resources and job postings for a career path detail page.
function loadCareerPathDetails(careerPathId) {
    $.getJSON('/CareerPaths/GetDetails', { id: careerPathId })
        .done(function (data) {
            $('#categoryName').text(data.category || 'N/A');
            renderResources(data.resources);
            renderJobs(data.jobs);
        })
        .fail(function () {
            $('#categoryName').text('N/A');
            $('#resourcesList').html('<p class="text-muted">Không thể tải tài nguyên.</p>');
            $('#jobsList').html('<p class="text-muted">Không thể tải tin tuyển dụng.</p>');
        });
}

function renderResources(resources) {
    var list = $('#resourcesList');
    list.empty();
    if (!resources || resources.length === 0) {
        list.html('<p class="text-muted">Chưa có tài nguyên học tập.</p>');
        return;
    }
    var ul = $('<ul class="list-group"></ul>');
    resources.forEach(function (r) {
        var label = siteEscapeHtml(r.title || 'Tài nguyên');
        var content = r.url
            ? '<a href="' + r.url + '" target="_blank" rel="noopener">' + label + '</a>'
            : label;
        if (r.resourceType) {
            content += ' <span class="badge bg-secondary">' + siteEscapeHtml(r.resourceType) + '</span>';
        }
        ul.append('<li class="list-group-item">' + content + '</li>');
    });
    list.append(ul);
}

function renderJobs(jobs) {
    var list = $('#jobsList');
    list.empty();
    if (!jobs || jobs.length === 0) {
        list.html('<p class="text-muted">Chưa có tin tuyển dụng.</p>');
        return;
    }
    jobs.forEach(function (j) {
        var card = $('<div class="card mb-2"></div>');
        var body = $('<div class="card-body py-2"></div>');
        body.append('<h6 class="mb-1">' + siteEscapeHtml(j.title) + '</h6>');
        if (j.companyName) {
            body.append('<div class="text-muted small">' + siteEscapeHtml(j.companyName) + '</div>');
        }
        if (j.salary) {
            body.append('<div class="small">Mức lương: ' + Number(j.salary).toLocaleString('vi-VN') + ' đ</div>');
        }
        card.append(body);
        list.append(card);
    });
}

function siteEscapeHtml(text) {
    if (text == null) return '';
    return $('<div></div>').text(text).html();
}
