function activarMenu(menu) {
    $("#menuPpal").find("li.active").removeClass("active");
    $("#menu" + menu).addClass("active");
}

// Responsables
var DlgResponsables = function () {
    this.Ver = function (tipo, id) {
        NProgress.start();
        NProgress.inc();
        window.LoadingIni();
        $.ajax({
            type: "POST",
            url: "/responsables/ver",
            data: { tipo: tipo, id: id }
        }).done(function (data) {
            NProgress.inc();
            // add dialog data
            $("#containerModal").html(data);
            // show dialog
            NProgress.inc();
            $("#DlgAbmResponsables").on("shown.bs.modal", function () {
                $("#respoBuscar").select2({
                    placeholder: "Ingresa algunas letras del Responsable...",
                    language: "es",
                    allowClear: true,
                    minimumInputLength: 2,
                    ajax: {
                        cache: true,
                        dataType: "json",
                        type: "GET",
                        delay: 250,
                        url: "Personas/Buscar",
                        data: function(searchTerm) {
                            return { q: searchTerm };
                        },
                        processResults: function(data /*, page*/) {
                            return {
                                results: data
                            };
                        }
                    }
                }).on("change", function (e) {
                    var respoId = $("#respoIdPersona");
                    var personaNombre = $("#personaNombre");
                    var personaTel = $("#personaTelefono");
                    var personaEmail = $("#personaEmail");
                    if (e.val) {
                            $.ajax({
                                type: "GET",
                                url: "Personas/BuscarPorId",
                                data: {
                                    id: e.val
                                }
                            }).done(function (persona) {
                                respoId.val(persona.Id);
                                personaNombre.html(persona.Nombre);
                                personaTel.html(persona.Telefono);
                                personaEmail.html(persona.Email);
                            }).fail(function (jqXhr, textStatus, errorThrown) {
                                alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
                            });
                    } else {
                        respoId.val("");
                        personaNombre.html("");
                        personaTel.html("");
                        personaEmail.html("");
                    }
                });
            }).modal("show");
            // remove dialog on close
            NProgress.inc();
            $("#DlgAbmResponsables").on("hidden.bs.modal", function () {
                Grid.Refresh();
                $(this).remove();
                $(".modal-scrollable").hide();
            });
            NProgress.done(true);
            window.LoadingFin();
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada a la Pantalla de Reponsables: " + textStatus + "_" + errorThrown);
        });
    }
};
var dlgResponsables = new DlgResponsables();

// Generico
var DlgModal = function () {
    this.Ver = function (url) {
        NProgress.start();
        NProgress.inc();
        window.LoadingIni();
        $.ajax({
            type: "GET",
            url: url
        }).done(function (data) {
            NProgress.inc();

            var permisos = data.search("Usted no posee permisos");

            if (permisos == -1)
            {
                // add dialog data
                $("#DlgGenericoBody").html(data);
                NProgress.inc();
                $("#DlgGenericoTitulo").html($("#Titulo").val() + " <small>" + $("#Subtitulo").val() + "</small>");
                // show dialog
                NProgress.inc();
                $("#DlgGenerico").modal({ backdrop: 'static', keyboard: false })
                $("#DlgGenerico").modal("show");
                NProgress.inc();
                // Tiene un Mapa en el Modal?
                $("#DlgGenerico").on("shown.bs.modal", function () {
                    var map, centro;
                
                    var zIndex = 1040 + (10 * $(".modal:visible").length);
                    $(this).css("z-index", zIndex);
                    setTimeout(function () {
                        $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
                    }, 0);
                });
                // remove dialog on close
                $("#DlgGenerico").on("hidden.bs.modal", function () {
                    $(this).remove();
                    $(".modal-scrollable").hide();
                    //TODO: Si se desa sacar el Refresco automático de la Grilla, comentar este bloque
                    if (Grid) {
                        setTimeout(function () {
                            Grid.Refresh();
                        }, 100);
                    }
                });
                NProgress.done(true);
                window.LoadingFin();
            }
            else
            {
                window.LoadingFin();
                NProgress.done(true);
                MensajeError("Usted no posee permisos para acceder a la función solicitada");
            }
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
        });
    }
};
var dlgModal = new DlgModal();


// Generico
var DlgModalCampos = function () {
    this.Ver = function (url) {
        //NProgress.start();
        //NProgress.inc();
        //window.LoadingIni();
        $.ajax({
            type: "GET",
            url: url
        }).done(function (data) {
            //NProgress.inc();
            // add dialog data
            $("#DlgGenericoCamposBody").html(data);
            // NProgress.inc();
            //$("#DlgGenericoTitulo").html($("#Titulo").val() + " <small>" + $("#Subtitulo").val() + "</small>");
            // show dialog
            // NProgress.inc();
            $("#DlgGenericoCampos").modal("show");
            //  NProgress.inc();
            // Tiene un Mapa en el Modal?
            //$("#DlgGenerico").on("shown.bs.modal", function () {
            //    //var map, centro;
            //    //// Chequea si tiene un Mapa en el Modal
            //    //if ($("#geocomplete").length > 0) {
            //    //    //console.log("Google Maps Resize()");
            //    //    // Hace un Resize() para refrescar el Mapa
            //    //    map = $("#geocomplete").geocomplete("map");
            //    //    centro = map.getCenter();
            //    //    window.google.maps.event.trigger(map, "resize");
            //    //    if (centro && (centro.lat() !== -35 && centro.lng() !== -65)) {
            //    //        map.setCenter(centro);
            //    //        map.setZoom(16);
            //    //    } else {
            //    //        centro = new window.google.maps.LatLng(-35.0, -65.0); // Centro del País
            //    //        map.setCenter(centro);
            //    //        map.setZoom(5);
            //    //    }
            //    //}
            //    //if (window.mapaInstituto) {
            //    //    map = window.mapaInstituto;
            //    //    centro = map.getCenter();
            //    //    window.google.maps.event.trigger(map, "resize");
            //    //    map.setCenter(centro);
            //    //    map.setZoom(16);
            //    //}
            //    // http://jsfiddle.net/CxdUQ/
            //    // http://stackoverflow.com/questions/19305821/multiple-modals-overlay
            //    var zIndex = 1040 + (10 * $(".modal:visible").length);
            //    $(this).css("z-index", zIndex);
            //    setTimeout(function () {
            //        $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
            //    }, 0);
            //});
            //// remove dialog on close
            //$("#DlgGenerico").on("hidden.bs.modal", function () {
            //    $(this).remove();
            //    $(".modal-scrollable").hide();
            //    //TODO: Si se desa sacar el Refresco automático de la Grilla, comentar este bloque
            //    if (Grid) {
            //        setTimeout(function () {
            //            Grid.Refresh();
            //        }, 100);
            //    }
            //});
            //NProgress.done(true);
            //window.LoadingFin();
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
        });
    }
};
var dlgModalCampos = new DlgModalCampos();


// Generico
var DlgModal2 = function () {
    this.Ver = function (url) {
        NProgress.start();
        NProgress.inc();
        window.LoadingIni();

        if (url.indexOf("?") == -1) {
            url = url + "?callback=AjaxOk2";
        } else {
            url = url + "&callback=AjaxOk2";
        }

        $.ajax({
            type: "GET",
            url: url + "&callback=AjaxOk2"
        }).done(function (data) {
            NProgress.inc();
            // add dialog data
            $("#DlgGenericoBody2").html(data);
            NProgress.inc();
            $("#DlgGenericoTitulo2").html($("#Titulo2").val() + " <small>" + $("#Subtitulo2").val() + "</small>");
            // show dialog
            NProgress.inc();
            $("#DlgGenerico2").modal({ backdrop: 'static', keyboard: false })
            $("#DlgGenerico2").on("show.bs.modal", ".modal", function () {
                var zIndex = 1040 + (10 * $(".modal:visible").length);
                $(this).css("z-index", zIndex);
                setTimeout(function() {
                    $(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
                }, 0);
            }).modal("show");
            NProgress.inc();
            NProgress.done(true);
            window.LoadingFin();
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
        });
    }
};
var dlgModal2 = new DlgModal2();

// Generico de tamaño mínimo
var DlgModalMin = function () {
    this.Ver = function (url) {
        NProgress.start();
        NProgress.inc();
        window.LoadingIni();
        $.ajax({
            type: "GET",
            url: url
        }).done(function (data) {
            NProgress.inc();
            // add dialog data
            $("#DlgGenericoMinBody").html(data);
            NProgress.inc();
            $("#DlgGenericoMinTitulo").html($("#Titulo").val() + " <small>" + $("#Subtitulo").val() + "</small>");
            // show dialog
            NProgress.inc();
            $("#DlgGenericoMin").modal("show");
            NProgress.inc();
            // remove dialog on close
            $("#DlgGenericoMin").on("hidden.bs.modal", function () {
                $(this).remove();
                $(".modal-scrollable").hide();
                setTimeout(function () {
                    Grid.Refresh();
                }, 100);
            });
            NProgress.done(true);
            window.LoadingFin();
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
        });
    }
};
var dlgModalMin = new DlgModalMin();

// Ficha
var DlgFicha = function () {
    this.Ver = function (url) {
        NProgress.start();
        NProgress.inc();
        window.LoadingIni();
        $.ajax({
            type: "GET",
            url: url
        }).done(function (data) {
            NProgress.inc();
            // add dialog data
            $("#DlgFichaBody").html(data);
            NProgress.inc();
            $("#DlgFichaTitulo").html($("#Titulo").val() + " <small>" + $("#Subtitulo").val() + "</small>");
            // show dialog
            NProgress.inc();
            $("#DlgFicha").on("shown.bs.modal", function() {
                // Hace un Resize() para refrescar el Mapa
                NProgress.inc();
                if (window.mapaInstituto) {
                    var map = window.mapaInstituto;
                    var centro = map.getCenter();
                    window.google.maps.event.trigger(map, "resize");
                    map.setCenter(centro);
                    map.setZoom(16);
                }              
            }).modal("show");
            NProgress.inc();
            // remove dialog on close
            $("#DlgFicha").on("hidden.bs.modal", function () {
                $(this).remove();
                $(".modal-scrollable").hide();
            });
            NProgress.done(true);
            window.LoadingFin();
        }).fail(function (jqXhr, textStatus, errorThrown) {
            window.LoadingFin();
            NProgress.done(true);
            alert("Error en la llamada: " + url + "\n\n" + textStatus + "\n\n" + errorThrown);
        });
    }
};
var dlgFicha = new DlgFicha();

$.fn.pressEnter = function (fn) {

    return this.each(function () {
        $(this).bind("enterPress", fn);
        $(this).keyup(function(e) {
            if (e.keyCode === 13) {
                $(this).trigger("enterPress");
            }
        });
    });
};