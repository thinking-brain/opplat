using System.Collections.Generic;

namespace TallerWebApi.Models {
    public class PaginadorGenerico<T> where T : class {
      /// <summary>
        /// Página devuelta por la consulta actual.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Número de registros de la página devuelta.
        /// </summary>
        public int ItemsPerPage { get; set; }
        /// <summary>
        /// Total de registros de consulta.
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Total de páginas de la consulta.
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Texto de búsqueda de la consuta actual.
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// Columna por la que esta ordenada la consulta actual.
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// Resultado segun Id.
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// Tipo de ordenación de la consulta actual: ASC o DESC.
        /// </summary>
        public string TypeOrder { get; set; }
        /// <summary>
        /// Resultado devuelto por la consulta a la tabla
        /// en función de todos los parámetros anteriores.
        /// </summary>
        public IEnumerable<T> Result { get; set; }
    }
}