import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export enum Gravite {
  Mineure = 0,
  Majeure = 1,
  Critique = 2,
} //TypeScript a lui aussi son propre système d'enum, syntaxiquement proche du C#. En assignant explicitement les valeurs numériques (= 0, = 1, = 2), on s'assure qu'elles correspondent exactement à l'ordre déclaré côté C# — c'est cette correspondance qui permet au JSON échangé ("gravite": 1) d'avoir le même sens des deux côtés.

export enum StatutNonConformite {
  Ouverte = 0,
  EnCours = 1,
  Cloturee = 2,
}

export interface Audit {
  id: number;
  reference: string;
  client: string;
  dateAudit: string;
}

export interface NonConformite {
  id: number;
  description: string;
  gravite: Gravite;
  statut: StatutNonConformite;
  dateEcheance: string;
  auditId: number;
  audit: Audit | null;
} //Reflète directement ce qu'on a vu dans la réponse API : chaque non-conformité contient un objet audit imbriqué (ou null, à cause d'IgnoreCycles quand on regarde depuis l'audit parent). Le typage Audit | null force à gérer explicitement ce cas dans le composant, plutôt que de risquer une erreur runtime en supposant que audit existe toujours.

@Injectable({
  providedIn: 'root',
})
export class NonConformitesService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5135/api/NonConformites';

  getAll(statut?: StatutNonConformite): Observable<NonConformite[]> {
    //statut?: StatutNonConformite : paramètre optionnel (le ? avant :) — on peut appeler getAll() sans rien, ou getAll(StatutNonConformite.Ouverte).
    let url = this.apiUrl;
    if (statut !== undefined) {
      url += `?statut=${StatutNonConformite[statut]}`; //syntaxe particulière des enums TypeScript — utiliser l'enum comme un objet indexé permet de retrouver le nom textuel à partir de la valeur numérique (ex: StatutNonConformite[0] renvoie "Ouverte"). C'est nécessaire ici car côté .NET, [FromQuery] StatutNonConformite? statut attend soit un nombre, soit le nom exact de l'enum en chaîne — les deux fonctionnent, mais le nom textuel rend l'URL plus lisible (?statut=Ouverte plutôt que ?statut=0).
    }
    return this.http.get<NonConformite[]>(url);
  }

  getById(id: number): Observable<NonConformite> {
    return this.http.get<NonConformite>(`${this.apiUrl}/${id}`);
  }

  create(nc: Partial<NonConformite>): Observable<NonConformite> {
    return this.http.post<NonConformite>(this.apiUrl, nc);
  }

  update(id: number, nc: NonConformite): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, nc);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
