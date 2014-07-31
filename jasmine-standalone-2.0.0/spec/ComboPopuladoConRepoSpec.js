describe("Un ComboPopuladoConRepo", function() {
  var populador_combos;
  var combo;
  var mock_repo

  beforeEach(function() {
	mock_repo = { buscar: function (nombre_repositorio, criterio, onSuccess, onError) {} }
	populador_combos = new ComboPopuladoConRepoBuilder(mock_repo);
	spyOn(mock_repo, 'buscar');
  });

  it("debe pedir los datos al repo", function() {
	combo = new SuperCombo({ui:$('<select id="combo_localidades">'),
							nombre_repositorio: "Localidades",
							repositorio: mock_repo});
							
	expect(mock_repo.buscar).toHaveBeenCalled();
	expect(mock_repo.buscar.calls.mostRecent().args[0]).toEqual("Localidades");
  });
  
  it("xxx", function() {
	var combos = populador_combos.construirCombosEn($('<div><select id="combo_localidades" dataProvider="Localidades"></select></div>'));
	expect(mock_repo.buscar).toHaveBeenCalled();
	expect(mock_repo.buscar.calls.mostRecent().args[0]).toEqual("Localidades");
	
  });
  
});